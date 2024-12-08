from flask import Flask, request, jsonify
import redis
import uuid
import logging
import sys

# Flask 애플리케이션 생성
app = Flask(__name__)

# 명령줄 인자 처리
def parse_args():
    if len(sys.argv) < 3:
        print("Usage: python main.py <server_port> <redis_port>")
        sys.exit(1)  # 프로그램 종료

    try:
        server_port = int(sys.argv[1])  # 첫 번째 매개변수: 서버 포트
        redis_port = int(sys.argv[2])  # 두 번째 매개변수: Redis 포트
        return server_port, redis_port
    except ValueError:
        print("Invalid port numbers. Please provide valid integers for server and Redis ports.")
        sys.exit(1)  # 프로그램 종료

# 포트 설정
server_port, redis_port = parse_args()

# Redis 클라이언트 초기화
redis_client = redis.StrictRedis(host='localhost', port=redis_port, decode_responses=True)

# 로깅 설정
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler("map-mamange.log"),
        logging.StreamHandler()
    ]
)

logger = logging.getLogger(__name__)

# POST /v1/map: JSON 데이터를 Redis에 저장
@app.route('/v1/map', methods=['POST'])
def save_to_redis():
    try:
        data = request.get_json()
        if not data:
            logger.warning("Received invalid JSON in POST request.")
            return jsonify({"error": "Invalid JSON"}), 400

        data_id = str(uuid.uuid4())
        redis_client.set(data_id, jsonify(data).get_data(as_text=True))
        logger.info(f"Data saved to Redis with ID: {data_id}")

        return jsonify({"id": data_id}), 201
    except Exception as e:
        logger.error(f"Error saving to Redis: {str(e)}")
        return jsonify({"error": str(e)}), 500

# GET /v1/map: Redis에 저장된 특정 데이터 또는 모든 데이터 가져오기
@app.route('/v1/map', methods=['GET'])
def get_from_redis():
    try:
        key = request.args.get('key')
        if key:
            value = redis_client.get(key)
            if value is None:
                logger.warning(f"No data found for key: {key}")
                return jsonify({"error": f"No data found for key: {key}"}), 404
            logger.info(f"Retrieved data for key: {key}")
            return jsonify([{"key": key, "data": value}]), 200

        keys = redis_client.keys()
        data = [
            {
                "key": k,
                "data": redis_client.get(k)
            }
            for k in keys
        ]
        logger.info(f"Retrieved {len(data)} items from Redis.")
        return jsonify(data), 200
    except Exception as e:
        logger.error(f"Error retrieving from Redis: {str(e)}")
        return jsonify({"error": str(e)}), 500

# DELETE /v1/map: Redis에서 특정 데이터 삭제
@app.route('/v1/map', methods=['DELETE'])
def delete_from_redis():
    try:
        key = request.args.get('key')
        if not key:
            logger.warning("No key provided for DELETE request.")
            return jsonify({"error": "Key is required to delete data"}), 400

        result = redis_client.delete(key)
        if result == 0:
            logger.warning(f"No data found for key: {key}")
            return jsonify({"error": f"No data found for key: {key}"}), 404

        logger.info(f"Deleted data for key: {key}")
        return jsonify({"message": f"Data for key '{key}' deleted successfully"}), 200
    except Exception as e:
        logger.error(f"Error deleting from Redis: {str(e)}")
        return jsonify({"error": str(e)}), 500

# 서버 실행
if __name__ == '__main__':
    print(f"Starting server on port {server_port} and connecting to Redis on port {redis_port}")
    app.run(host='0.0.0.0', port=server_port)
