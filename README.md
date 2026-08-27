# 🖨️ SmartPrinter

SmartPrinter is a **local network printing system** that allows users to print PDF documents through a web browser.

A printer can be connected to one computer, while other devices on the same network can use SmartPrinter to upload and print documents.

## 📌 How It Works

```text
Laptop / Phone
      │
      ▼
SmartPrinterWeb
   (Port 5170)
      │
      │ HTTP API
      ▼
SmartPrinterServer
      │
      ▼
  Print Queue
      │
      ▼
    Printer
```

The project contains two applications:

* **SmartPrinterWeb** – Blazor web interface for uploading PDFs, previewing documents, selecting a printer, and choosing copies.
* **SmartPrinterServer** – ASP.NET Core backend that manages print jobs, printers, database, and the background print worker.

---

## 🛠️ Technologies

* C#
* .NET 10
* ASP.NET Core
* Blazor
* Entity Framework Core
* SQLite
* Docker / Docker Compose
* Windows Printing
* Windows Services

---

## 📂 Project Structure

```text
SmartPrinter/
│
├── SmartPrinterServer/    # Backend / API
├── SmartPrinterWeb/       # Blazor Web App
├── docker-compose.yaml
└── README.md
```

---

# 🚀 Development Setup

## 1. Clone the Repository

```bash
git clone https://github.com/jahanzaibkhan1995/SmartPrinter.git
cd SmartPrinter
```

## 2. Restore and Build

```bash
dotnet restore
dotnet build
```

## 3. Start the Server

```bash
cd SmartPrinterServer
dotnet run
```

## 4. Start the Web App

Open another terminal:

```bash
cd SmartPrinterWeb
dotnet run
```

Then open:

```text
http://localhost:5170
```

---

# 🌐 Using From Another Device

Find the IP address of the computer running SmartPrinter:

```powershell
ipconfig
```

For example:

```text
192.168.0.102
```

From another device on the same network:

```text
http://192.168.0.102:5170
```

Make sure the required port is allowed through Windows Firewall.

---

# 🖨️ Printer Setup

The printer must be installed on the computer running `SmartPrinterServer`.

Check installed printers:

```powershell
Get-Printer
```

The server uses the Windows printing system to communicate with the configured printer.

---

# 📄 Printing Workflow

1. Open SmartPrinter.
2. Select a PDF.
3. Preview the PDF.
4. Select a printer.
5. Select the number of copies.
6. Click **Print Document**.
7. The server creates a print job.
8. `PrintWorker` processes the job.
9. The document is sent to the printer.

---

# 🔌 API

| Method | Endpoint                   | Purpose                 |
| ------ | -------------------------- | ----------------------- |
| GET    | `/api/health`              | Check server status     |
| GET    | `/api/printers/configured` | Get configured printers |
| GET    | `/api/printjobs`           | Get print jobs          |
| GET    | `/api/printjobs/{id}`      | Get a specific job      |
| DELETE | `/api/printjobs/{id}`      | Cancel a job            |
| POST   | `/api/printjobs`           | Create a print job      |

---

# 📦 Publishing the Application

For deployment, publish the applications instead of running them with `dotnet run`.

## Publish SmartPrinterServer

From the solution directory:

```powershell
dotnet publish SmartPrinterServer/SmartPrinterServer.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o publish/Server
```

The published server will be placed in:

```text
publish/
└── Server/
```

## Publish SmartPrinterWeb

```powershell
dotnet publish SmartPrinterWeb/SmartPrinterWeb.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o publish/Web
```

The result will be:

```text
publish/
├── Server/
└── Web/
```

### Why publish?

Publishing creates a deployment-ready version of the application.

Instead of:

```text
dotnet run
```

you can run the published application directly:

```powershell
.\SmartPrinterServer.exe
```

and:

```powershell
.\SmartPrinterWeb.exe
```

Using `--self-contained true` also includes the required .NET runtime, so the target Windows computer does not need to have the .NET runtime separately installed.

---

# 🪟 Running SmartPrinterServer as a Windows Service

For a permanent printer server, `SmartPrinterServer` can run as a **Windows Service**.

This means:

* The server starts automatically with Windows.
* You do not need to keep PowerShell open.
* The server can run in the background.
* The printer service is always available.

## 1. Publish the Server

First publish the server:

```powershell
dotnet publish SmartPrinterServer/SmartPrinterServer.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o E:\SmartPrinter\Server
```

You should then have:

```text
E:\SmartPrinter\Server\
│
├── SmartPrinterServer.exe
├── appsettings.json
├── *.dll
└── other required files
```

## 2. Create the Windows Service

Run PowerShell as **Administrator**:

```powershell
sc.exe create SmartPrinterServer `
    binPath= "E:\SmartPrinter\Server\SmartPrinterServer.exe" `
    start= auto
```

## 3. Start the Service

```powershell
sc.exe start SmartPrinterServer
```

Check its status:

```powershell
sc.exe query SmartPrinterServer
```

If successful, the server runs in the background.

---

# 🛑 Managing the Windows Service

Stop:

```powershell
sc.exe stop SmartPrinterServer
```

Start:

```powershell
sc.exe start SmartPrinterServer
```

Restart:

```powershell
sc.exe stop SmartPrinterServer
sc.exe start SmartPrinterServer
```

Check status:

```powershell
sc.exe query SmartPrinterServer
```

Remove the service:

```powershell
sc.exe delete SmartPrinterServer
```

> **Important:** Stop the service before deleting it.

---

# 🔄 Recommended Production Setup

For a computer that is permanently connected to the printer:

```text
Windows PC
│
├── SmartPrinterServer
│       └── Windows Service
│
├── SmartPrinterWeb
│       └── Web Application
│
└── Physical Printer
        └── USB / Network
```

The server can then remain running even when no PowerShell window is open.

---

# 🐳 Docker

Build:

```bash
docker compose build
```

Start:

```bash
docker compose up
```

Run in background:

```bash
docker compose up -d
```

Stop:

```bash
docker compose down
```

> **Note:** When the printer is connected through USB to Windows, the actual printing component may need to run directly on Windows. Docker is more suitable for the web/API components unless printer access is specifically configured.

---

# 🔧 Troubleshooting

### Connection refused

Check whether the server is running:

```powershell
netstat -ano | findstr LISTENING
```

### Cannot access from another device

Check:

* Both devices are on the same network.
* Use the server IP instead of `localhost`.
* Windows Firewall allows the required port.
* The application is listening on the correct network interface.

### PDF preview not working

Make sure the browser can reach the backend API and that the API URL points to the **server computer**, not `localhost` on the client device.

### Port already in use

```powershell
netstat -ano | findstr :5170
```

Find the process using the returned PID:

```powershell
tasklist /FI "PID eq <PID>"
```

---

# 🔮 Future Improvements

* User authentication
* Multiple printer support
* Print history
* Printer status
* Job retry
* Real-time job status
* Duplex printing
* Page selection
* Admin dashboard
* HTTPS

---

# 👤 Author

**Jahanzaib Khan**

GitHub:
https://github.com/jahanzaibkhan1995

Repository:
https://github.com/jahanzaibkhan1995/SmartPrinter

https://github.com/jahanzaibkhan1995/SmartPrinter
