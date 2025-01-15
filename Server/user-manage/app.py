from flask import Flask
from flask_restx import Api, Resource, fields
from database import create_session_factory, load_config
from models import User, UserToken
import uuid
from datetime import datetime, timedelta

app = Flask(__name__)
api = Api(app, 
    title='User Management API',
    description='사용자 관리를 위한 API',
    doc='/docs'
)

# API 모델 정의
auth_request = api.model('AuthRequest', {
    'platformType': fields.Integer(required=True, description='플랫폼 타입'),
    'platformId': fields.String(required=True, description='플랫폼 ID')
})

auth_response = api.model('AuthResponse', {
    'status': fields.Integer(description='상태 코드'),
    'message': fields.String(description='응답 메시지'),
    'data': fields.Nested(api.model('AuthData', {
        'userIdx': fields.Integer(description='사용자 ID'),
        'token': fields.String(description='발급된 토큰')
    }))
})

# DB 세션 팩토리 생성
SessionFactory, engine = create_session_factory()

@api.route('/v1/user/register')
class Register(Resource):
    @api.doc('사용자 등록')
    @api.expect(auth_request)
    @api.response(200, '성공', auth_response)
    @api.response(400, '잘못된 요청')
    @api.response(409, '중복된 사용자')
    def post(self):
        """새로운 사용자를 등록합니다."""
        session = SessionFactory()
        try:
            data = api.payload
            print("Received data:", data)
            
            platformType = data.get('platformType')
            platformId = data.get('platformId')
            
            print("Parsed values:", platformType, platformId)
            
            if platformType is None or platformId is None:
                return {
                    "status": 400,
                    "message": "필수 파라미터가 누락되었습니다",
                    "data": None
                }, 400
            
            existing_user = session.query(User).filter_by(
                platform_type=platformType,
                platform_id=platformId
            ).first()
            
            if existing_user:
                return {
                    "status": 409,
                    "message": "이미 존재하는 사용자입니다",
                    "data": None
                }, 409
            
            new_user = User(
                platform_type=platformType,
                platform_id=platformId
            )
            session.add(new_user)
            session.flush()
            
            token = str(uuid.uuid4())
            new_token = UserToken(
                user_id=new_user.id,
                token=token,
                expire_at=datetime.now() + timedelta(days=30)
            )
            session.add(new_token)
            session.commit()
            
            return {
                "status": 200,
                "message": "success",
                "data": {
                    "userIdx": new_user.id,
                    "token": token
                }
            }
            
        except Exception as e:
            session.rollback()
            return {
                "status": 500,
                "message": str(e),
                "data": None
            }, 500
        finally:
            session.close()

@api.route('/v1/user/login')
class Login(Resource):
    @api.doc('사용자 로그인')
    @api.expect(auth_request)
    @api.response(200, '성공', auth_response)
    @api.response(400, '잘못된 요청')
    @api.response(404, '사용자 없음')
    def post(self):
        """기존 사용자 로그인을 처리합니다."""
        session = SessionFactory()
        try:
            data = api.payload
            print("Received data:", data)
            
            platformType = data.get('platformType')
            platformId = data.get('platformId')
            
            print("Parsed values:", platformType, platformId)
            
            if platformType is None or platformId is None:
                return {
                    "status": 400,
                    "message": "필수 파라미터가 누락되었습니다",
                    "data": None
                }, 400
            
            existing_user = session.query(User).filter_by(
                platform_type=platformType,
                platform_id=platformId
            ).first()
            
            if not existing_user:
                return {
                    "status": 404,
                    "message": "존재하지 않는 사용자입니다",
                    "data": None
                }, 404
            
            token = str(uuid.uuid4())
            new_token = UserToken(
                user_id=existing_user.id,
                token=token,
                expire_at=datetime.now() + timedelta(days=30)
            )
            session.add(new_token)
            session.commit()
            
            return {
                "status": 200,
                "message": "success",
                "data": {
                    "userIdx": existing_user.id,
                    "token": token
                }
            }
            
        except Exception as e:
            session.rollback()
            return {
                "status": 500,
                "message": str(e),
                "data": None
            }, 500
        finally:
            session.close()

if __name__ == '__main__':
    server_port, *_ = load_config()
    app.run(port=server_port, debug=True) 