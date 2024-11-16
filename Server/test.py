import socket

host = "0.0.0.0"  # 서버의 IP 주소 또는 도메인 이름
port = 38201       # 포트 번호

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((host, port))
server_socket.listen(5)

print(f'서버가 {host}:{port}에서 대기 중입니다...')

while True:
    client_socket, client_address = server_socket.accept()
    print(f'클라이언트 {client_address}가 연결되었습니다.')
    
    try:
        data = client_socket.recv(1024).decode('utf-8')
        if not data:
            continue

        parts = data.split('&&')
        if len(parts) != 0:
            name = parts[0]
            message = parts[1]
            response = f'테스트 서버에 어서 오세요, {name}. 보내신 메시지는 \"{message}\" 입니다.'

            print(f"클라이언트 이름: {name}")
            print(f"클라이언트 메시지: {message}")
        else:
            response = "유효하지 않은 요청"

        client_socket.send(response.encode('utf-8'))

    except Exception as e:
        print(f'오류 발생: {e}')

    finally:
        print('연결종료')
        client_socket.close()