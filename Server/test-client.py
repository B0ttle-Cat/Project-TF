import socket

server_address = '182.213.92.175'
server_port = 38201

client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect((server_address, server_port))

name = input('이름을 입력 하세요 >> ')

message = input('메시지를 입력 하세요 >> ')
request = f'{name}&&{message}'
client_socket.send(request.encode('utf-8'))
response = client_socket.recv(1024).decode('utf-8')
print(f'{name} : {message}')
print(f'서버 : {response}\n')

client_socket.close()