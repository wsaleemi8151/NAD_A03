# programmers - Gursharan Singh - Waqar Ali Saleemi
# file - LoggingClient.py
import socket
from sys import argv
from xml.etree.ElementTree import tostring

target_host = input("Enter the host IP: ")
target_port = input("Enter the target port: ")
target_port = int(target_port)
great = "ok good"
msg = great

# create a socket connection
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# let the client connect
client.connect((target_host, target_port))

Fcode = 0

# loop until user choose to
while(Fcode != -1):

    # get user input for Facility code
    Fcode = 0
    Fcode = input("Enter the Facility code[0-23]or [-1 to exit]: ")
    Fcode = int(Fcode)

    if(Fcode == -1):
        break

    # get user input for severity level code
    Lcode = 0
    Lcode = input("Enter the severity Level[0-7]: ")
    Lcode = int(Lcode)

    Pcode = int(Fcode*8 + Lcode)

    Pcode = str(Pcode)

    Message = input("Enter the message to be sent: ")

    # message to be sent
    msg = Pcode + "|" + Message

    # send some data
    client.sendall(msg.encode())

    # get some data
    response = client.recv(4096)

    # server response
    print(response.decode())
