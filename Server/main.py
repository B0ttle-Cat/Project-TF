# main.py
import sys
import os
import logging
from server import MainServer

def setup_logging(log_file="log.txt"):
    # 로그 파일의 절대 경로 (main.py와 동일 위치)
    script_dir = os.path.dirname(os.path.abspath(sys.argv[0]))
    log_path = os.path.join(script_dir, log_file)

    # 로거 설정
    logging.basicConfig(
        level=logging.INFO,
        handlers=[
            logging.FileHandler(log_path, encoding="utf-8")
        ],
        format='[%(asctime)s] %(message)s', 
        datefmt='%Y-%m-%d %H:%M:%S'
    )
    
    # 콘솔 출력도 유지
    console_handler = logging.StreamHandler()
    console_handler.setLevel(logging.INFO)
    console_handler.setFormatter(logging.Formatter('%(message)s'))
    
    # 루트 로거에 핸들러 추가
    logging.getLogger().addHandler(console_handler)
    
    return log_path

def is_valid_port(port):
    try:
        # 정수 변환 시도
        port = int(port)
        
        # 포트 범위 체크 (1024-65535)
        return 1024 <= port <= 65535
    
    except (ValueError, TypeError):
        # 정수로 변환 불가능한 경우
        return False

def main():
    # 로깅 설정
    log_path = setup_logging()
    logging.info("=== 서버 시작 ===")
    logging.info(f"로그 파일 경로: {log_path}")

    # 시스템 인자로 포트 입력 확인
    if len(sys.argv) < 2:
        logging.error("사용법: python main.py <포트번호>")
        logging.error("포트 번호는 1024-65535 사이의 값이어야 합니다.")
        sys.exit(1)

    input_port = sys.argv[1]
    
    # 포트 유효성 검사
    if not is_valid_port(input_port):
        logging.error(f"유효하지 않은 포트 번호: {input_port}")
        logging.error("포트 번호는 1024-65535 사이의 정수여야 합니다.")
        sys.exit(1)

    # 유효한 포트로 서버 시작
    port = int(input_port)
    logging.info(f"서버를 포트 {port}에서 시작합니다.")

    server = MainServer(port=port)
    
    try:
        server.start()
    except KeyboardInterrupt:
        logging.info("\n서버를 종료합니다.")
        server.stop()

if __name__ == "__main__":
    main()