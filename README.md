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

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/jahanzaibkhan1995/SmartPrinter.git
cd SmartPrinter
```

### 2. Restore and build

```bash
dotnet restore
dotnet build
```

### 3. Start the Server

```bash
cd SmartPrinterServer
dotnet run
```

### 4. Start the Web App

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

## 🌐 Using From Another Device

Find the IP address of the computer running SmartPrinter:

```powershell
ipconfig
```

For example:

```text
192.168.0.102
```

Then open this on another device connected to the same network:

```text
http://192.168.0.102:5170
```

Make sure the required port is allowed through Windows Firewall.

---

## 🖨️ Printer Setup

The printer must be installed on the computer running `SmartPrinterServer`.

Check installed printers:

```powershell
Get-Printer
```

SmartPrinterServer uses the configured Windows printer to process print jobs.

---

## 📄 Printing Workflow

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

## 🔌 API

Some important endpoints:

| Method | Endpoint                   | Purpose                 |
| ------ | -------------------------- | ----------------------- |
| GET    | `/api/health`              | Check server status     |
| GET    | `/api/printers/configured` | Get configured printers |
| GET    | `/api/printjobs`           | Get print jobs          |
| GET    | `/api/printjobs/{id}`      | Get a specific job      |
| DELETE | `/api/printjobs/{id}`      | Cancel a job            |
| POST   | `/api/printjobs`           | Create a print job      |

---

## 🐳 Docker

Build the project:

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

> **Note:** If the printer is connected through USB to Windows, running the actual printing component inside a Linux Docker container may require additional configuration. Running the print server directly on Windows is usually simpler.

---

## 🔧 Troubleshooting

### Connection refused

Check whether the server is running and verify its port:

```powershell
netstat -ano | findstr LISTENING
```

### Cannot access from another device

Check:

* Both devices are on the same network.
* You are using the server's IP address instead of `localhost`.
* Windows Firewall allows the application port.
* The web application is listening on a network-accessible address.

### PDF preview not working

Make sure the browser can reach the backend API and that the API URL points to the **server computer**, not `localhost` on the client device.

---

## 🔮 Future Improvements

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

## 👤 Author

**Jahanzaib Khan**

GitHub:
https://github.com/jahanzaibkhan1995

Repository:
https://github.com/jahanzaibkhan1995/SmartPrinter
