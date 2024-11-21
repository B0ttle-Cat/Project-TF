# server.py
import socket
import logging

class MainServer:
    def __init__(self, port=38201):
        self.host = "0.0.0.0"
        self.port = port
        self.server_socket = None

    def start(self):
        try:
            self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.server_socket.bind((self.host, self.port))
            self.server_socket.listen(5)
            
            logging.info(f'서버가 {self.host}:{self.port}에서 대기 중입니다...')

            while True:
                try:
                    client_socket, client_address = self.server_socket.accept()
                    logging.info(f'클라이언트 {client_address}가 연결되었습니다.')
                    
                    data = client_socket.recv(1024).decode('utf-8')
                    if not data:
                        logging.warning('수신된 데이터가 없습니다.')
                        client_socket.close()
                        continue

                    parts = data.split('&&')
                    if len(parts) == 2:
                        name = parts[0]
                        message = parts[1]
                        response = f'테스트 서버에 어서 오세요, {name}. 보내신 메시지는 \"{message}\" 입니다.'
                        
                        logging.info(f"메시지 수신. name: {name}, message: {message}")
                    else:
                        response = "유효하지 않은 요청"
                        logging.warning(f"유효하지 않은 요청: {data}")

                    client_socket.send(response.encode('utf-8'))

                except Exception as e:
                    logging.error(f'클라이언트 처리 중 오류 발생: {e}')

                finally:
                    logging.info('연결종료')
                    client_socket.close()

        except Exception as e:
            logging.critical(f'서버 시작 중 致命的 오류 발생: {e}')
            self.stop()

    def stop(self):
        if self.server_socket:
            logging.info('서버를 종료합니다.')
            self.server_socket.close()