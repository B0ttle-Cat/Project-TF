from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base
import configparser
import sys

Base = declarative_base()

def load_config(config_file="config.ini"):
    config = configparser.ConfigParser()
    config.read(config_file)

    try:
        server_port = int(config["server"]["server_port"])
        mysql_ip = config["mysql"]["mysql_ip"]
        mysql_port = int(config["mysql"]["mysql_port"])
        user = config["mysql"]["user"]
        password = config["mysql"]["password"]
        database = config["mysql"]["database"]
        return server_port, mysql_ip, mysql_port, user, password, database
    except KeyError as e:
        print(f"환경 파일의 키를 찾을 수 없습니다: {e}")
        sys.exit(1)
    except ValueError as e:
        print(f"환경 파일의 값 형식이 올바르지 않습니다: {e}")
        sys.exit(1)

def create_session_factory():
    _, mysql_ip, mysql_port, user, password, database = load_config()
    
    DATABASE_URL = f"mysql+mysqlconnector://{user}:{password}@{mysql_ip}/{database}"
    
    engine = create_engine(
        DATABASE_URL,
        pool_size=5,
        pool_recycle=3600,  # 1시간마다 연결 재생성
        pool_pre_ping=True  # 연결 확인 후 끊어졌으면 재연결
    )
    
    SessionFactory = sessionmaker(bind=engine)
    return SessionFactory, engine 