import socket
import base64
import json

class ConnectionHandler:
    def __init__(self):
        self.game_client_socket = None
        self.train_socket = None

    def handle_data(self, sock, data):
        decoded_data = data.decode()
        if decoded_data == "/REGISTER-GAME-CLIENT":
            self.game_client_socket = sock
            self.game_client_socket.send(b'/REGISTER-GAME-CLIENT')

        elif decoded_data.find("/ENV-STATE") != -1:
            json_data = decoded_data.split("/ENV-STATE")[1]

            if (self.train_socket):
                self.train_socket.send(json_data.encode())
        
        elif decoded_data == "/GET-ENV-STATE":
            if (self.game_client_socket):
                self.game_client_socket.send(b'/GET-ENV-STATE')

        elif decoded_data == "/REGISTER-TRAIN-CLIENT":
            self.train_socket = sock
            self.train_socket.send(b'/REGISTER-TRAIN-CLIENT')

        elif decoded_data == "/GAME-OVER":
            if (self.train_socket):
                self.train_socket.send(b'/GAME-OVER')

        elif decoded_data.find("/ACTION") != - 1:
            self.game_client_socket.send(data)


    def get_game_client_socket(self):
        return self.game_client_socket

    def get_train_socket(self):
        return self.train_socket
