import socket
import sys
import logging

def is_valid_ip(ip):
    try:
        parts = ip.split('.')
        return (len(parts) == 4 and 
                all(0 <= int(part) <= 255 for part in parts))
    except (ValueError, TypeError):
        return False

def is_valid_port(port):
    try:
        port = int(port)
        return 1024 <= port <= 65535
    except (ValueError, TypeError):
        return False

def main():

    logging.basicConfig(level=logging.DEBUG)

    # 시스템 인자 검증
    if len(sys.argv) < 3:
        logging.critical("사용법: python client.py <IP주소> <포트번호>")
        logging.critical("IP주소는 유효한 IPv4 형식이어야 하고,")
        logging.critical("포트 번호는 1024-65535 사이의 값이어야 합니다.")
        sys.exit(1)

    # IP 주소 검증
    server_address = sys.argv[1]
    if not is_valid_ip(server_address):
        logging.critical(f"유효하지 않은 IP 주소: {server_address}")
        logging.critical("올바른 IPv4 형식(예: 127.0.0.1)으로 입력해주세요.")
        sys.exit(1)

    # 포트 번호 검증
    try:
        server_port = int(sys.argv[2])
        if not is_valid_port(server_port):
            logging.critical(f"유효하지 않은 포트 번호: {server_port}")
            logging.critical("포트 번호는 1024-65535 사이의 정수여야 합니다.")
            sys.exit(1)
    except ValueError:
        logging.critical(f"포트 번호는 정수여야 합니다.")
        sys.exit(1)

    # 소켓 연결
    try:
        client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client_socket.connect((server_address, server_port))
    except ConnectionRefusedError:
        logging.critical(f"서버 {server_address}:{server_port}에 연결할 수 없습니다.")
        sys.exit(1)

    # 사용자 입력
    name = input('이름을 입력 하세요 >> ')
    message = input('메시지를 입력 하세요 >> ')
    
    # 메시지 전송
    request = f'{name}&&{message}'
    client_socket.send(request.encode('utf-8'))
    
    # 응답 수신
    response = client_socket.recv(1024).decode('utf-8')
    logging.debug(f'{name} : {message}')
    logging.debug(f'서버 : {response}\n')

    # 소켓 종료
    client_socket.close()

if __name__ == "__main__":
    main()