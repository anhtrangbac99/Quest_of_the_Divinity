import socket
import types
import selectors
from connection_handler import ConnectionHandler

HOST = '127.0.0.1'
PORT = 27972

BUFFER_SIZE = 4096

sel = selectors.DefaultSelector()

sockets = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sockets.bind((HOST, PORT))
sockets.listen()
sockets.setblocking(False)

sel.register(sockets, selectors.EVENT_READ, data=None)
print("The server is listening at host: {}, port {}".format(HOST, PORT))

_connection_handler = ConnectionHandler()
####################################
def accept_wrapper(sock):
    connection, address = sock.accept()
    print("Connection established: {}".format(address))
    connection.setblocking(False)

    data = types.SimpleNamespace(addr=address, inb=b'', outb=b'')
    events = selectors.EVENT_READ | selectors.EVENT_WRITE
    sel.register(connection, events, data=data)

def service_connection(key, mask):
    sock = key.fileobj
    data = key.data

    if mask & selectors.EVENT_READ:
        recv_data = sock.recv(BUFFER_SIZE)
        if recv_data:
            data.outb += recv_data
        else:
            print("Closing connection with {}".format(data.addr))
            sel.unregister(sock)
            sock.close()
    if mask & selectors.EVENT_WRITE:
        if data.outb:
            _connection_handler.handle_data(sock, data.outb)

            data.outb = data.outb[len(data.outb):]
####################################

while True:
    events = sel.select(timeout=None)
    for key, mask in events:
        if key.data is None:
            accept_wrapper(key.fileobj)
        else:
            service_connection(key, mask)

