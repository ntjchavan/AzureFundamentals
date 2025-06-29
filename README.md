# AzureFundamentals
Azure projects

We will use Microsoft Azure Storage Explorer to check container/blob has created or not.
We will use local account, so that we don't need to use Azure Storage account

Open Visual Studio Code
Install Azurite Extension in Visual Studio Code.

Open the Command Palette (Ctrl+Shift+P on windows or Cmd+Shift+P on macOS)

Type - Azurite: Start

You can see that blob server has started on VS Code taskbar
http://127.0.0.1:10000

Now, open Az Storage Explorer -> Expand: Emulator & Attached -> Expand: Storage Account -> Expand (Emulator - Default Ports) (Key).

You will see storage type such as Blob Container, Queues, Tables.

Expand: Blob Containers, you will not see any containers.

Now, right click on project "AzureStorageBlob" and select set as StartUp Project

Run application.

This application is using .net9.0. So it doesn't support swagger.

Click on "View" in Visual Studio -> Click on "Other Windows" -> Click on "Endpoint Explorer".

You will see number of API endpoints which has there in application.

Right click on Endpoint -> Click on Generate Request -> it will open AzureStorageBlob.http file.

You will see your Endpoint.

You can add parameters which require for API endpoints & click on Send request (above of API endpoint)


https://localhost:7036/api/container/create-container/test1 HTTP/1.1


http://localhost:5033/api/Fileupload/createcontainer?containerName=test

https://localhost:7036/api/container/containert?containerName=testnew

POST http://localhost:5148/api/container/create-container/test

https://localhost:7036/api/fileupload/createcontainer?containerName=test 
http://localhost:7036/api/Fileupload/createcontainer?containerName=test
