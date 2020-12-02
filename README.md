# MailkitMemIssue
Simple app to reproduce Mailkit Mem Issue

To build the docker image run the following command in the consoleapp/MailKitConsoleApp folder

docker build -t consoleapp:latest .

To run the container run

docker run -it --env-file env.list consoleapp:latest

The env.list file specifies appsettings. The file's content should be

AppSettings__Smtp__SecureSocketOptions=None
AppSettings__Smtp__Host=<host-name>
AppSettings__Smtp__Port=<portNumber>
AppSettings__Smtp__UserName=<userName>
AppSettings__Smtp__Password=<password>

The env.list file contains sensitive information and should not be added to the repository

Running the container like this in interactive mode gives you a bash commandline.
From here you can run the application using ./MailkitconsoleApp

The container has gnu screen installed to run the app in one screen and analyze it in another.
