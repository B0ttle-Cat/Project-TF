from flask import Flask, request, jsonify
import logging
import sys
import mysql.connector
from mysql.connector import Error
import configparser

# 설정 파일 읽기 함수
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

def connect_to_mysql(host_ip: str, user: str, password: str, database: str):
    # MySQL 연결 설정
    connection_config = {
        "host": host_ip,
        "user": user,
        "password": password,
        "database": database,
    }
    
    try:
        # MySQL 연결
        conn = mysql.connector.connect(**connection_config)
        if conn.is_connected():
            print("MySQL 데이터베이스에 성공적으로 연결되었습니다.")
            return conn
    except Error as e:
        print(f"MySQL 연결 오류: {e}")
        return None


def fetch_users(mysql_ip, user, password, database):
    conn = connect_to_mysql(mysql_ip, user, password, database)
    if conn:
        try:
            cursor = conn.cursor()
            query = "SELECT * FROM users;"
            cursor.execute(query)
            rows = cursor.fetchall()
            
            print("users 테이블의 데이터:")
            for row in rows:
                print(row)
        except Error as e:
            print(f"쿼리 실행 오류: {e}")
        finally:
            conn.close()

def parse_args():
    if len(sys.argv) < 2:
        print("Usage: python main.py <config_file_path>")
        sys.exit(1)  # 프로그램 종료

    config_file = sys.argv[1]  # 첫 번째 명령줄 인자
    return config_file

if __name__ == "__main__":
    config_file = parse_args()

    server_port, mysql_ip, mysql_port, user, password, database = load_config(config_file)

    fetch_users(mysql_ip, user, password, database)