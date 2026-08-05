# 🏨 Hotel Management System (Otel Yönetim Sistemi)

A full-stack, enterprise-ready **Hotel Management System** built with a modern **.NET 10 Web API** backend and a responsive **Angular 21** frontend. The system provides secure JWT authentication, role-based access control (Admin & Customer), seamless online room reservations for customers, and a comprehensive administrative management dashboard for hotel staff.

---

## 🚀 Key Features

### 🔐 Authentication & Authorization (New)
- **JWT (JSON Web Token) Security**: Token-based authentication generated upon login and validated across secure API requests.
- **User Registration & Login (`/login`)**: Interactive view supporting both user login and account registration with password visibility toggle.
- **Role-Based Access Control (RBAC)**: Supports `admin` and `customer` roles with custom route guards (`adminGuard`, `authGuard`, `loginGuard`).
- **HTTP Interceptor (`authInterceptor`)**: Automatically injects JWT Bearer tokens into outgoing HTTP requests for seamless authorized communication.
- **Dynamic Header & Session Management**: Responsive header component reflecting current user login status, role, and quick logout functionality.

### 👤 Customer Portal (`/customer`)
- **Real-Time Room Availability Search**: Filter available rooms by specifying check-in and check-out dates.
- **Instant Price Calculation**: Automatic total price computation based on total nights stayed and nightly room rates.
- **Online Reservation Booking**: Guests can easily select rooms and complete bookings linked to registered user accounts.
- **Validation & Date Constraints**: Built-in validation preventing past date selection or invalid check-out ranges.

### 🛡️ Admin Management Dashboard (`/admin`)
- **Room Management (CRUD)**:
  - **Create**: Add new hotel rooms with room numbers, room types (Single, Double, Suite, etc.), and nightly prices.
  - **Update**: Edit existing room details (`Oda Numarası`, `Tip`, and `Gecelik Fiyat`) via an interactive Pop-up Modal.
  - **Delete**: Remove rooms from the system.
- **Occupancy Protection (`durum === 1`)**:
  - Rooms currently reserved (`durum === 1`) are protected against accidental updates or deletions.
  - Action buttons (`Güncelle` and `Odayı Sil`) are automatically **disabled** and **faded** (`opacity: 0.45`, `pointer-events: none`).
- **Reservation Oversight**: View all active and historical bookings complete with customer names, assigned rooms, stay dates, and total charges.
- **Booking Cancellation**: Instantly cancel or remove customer reservations.
- **Availability Monitoring**: Search and verify room availability directly from the admin workspace.
- **Modern Pop-up UI & Error Handling**:
  - **Glassmorphism Backdrop Blur**: Pop-up dialogs with blurred backdrops (`backdrop-filter: blur(6px)`).
  - **Smooth CSS Animations**: Entrance keyframe scaling and slide animations (`animate-popup`).
  - **Custom Error Alert Modal**: Reusable pop-up dialog for friendly error notification displays.

---

## 🛠️ Technology Stack

| Layer | Technology / Framework | Key Packages & Tools |
|---|---|---|
| **Backend** | .NET 10 Web API (C#) | `Microsoft.AspNetCore.Authentication.JwtBearer`, `MySql.Data` (v9.7.0), `Swashbuckle.AspNetCore` (Swagger) |
| **Frontend** | Angular 21 (Standalone Components) | TypeScript, RxJS, Angular Forms, Angular Router, Vitest |
| **Authentication** | JWT (JSON Web Tokens) | `System.IdentityModel.Tokens.Jwt`, LocalStorage Session Management |
| **Database** | MySQL RDBMS | Raw SQL execution via Repository Pattern |
| **API Protocol** | RESTful JSON API | Swagger OpenAPI UI, CORS enabled for Angular |

---

## 🏗️ Architecture & System Workflow

The project follows a clean **Repository Pattern** on the backend to isolate business logic from database interactions, paired with modular **Angular Standalone Components**, route guards, and HTTP interceptors on the frontend.

```mermaid
graph TD
    subgraph Frontend [Angular 21 UI]
        LP[Login & Register Component]
        CP[Customer Portal Component]
        AP[Admin Dashboard Component]
        AG[Auth & Admin Route Guards]
        AI[Auth HTTP Interceptor]
        AS[Auth Service]
        ApiS[API Service]

        LP -->|Login / Register| AS
        AG -->|Protect Routes| CP
        AG -->|Protect Routes| AP
        AI -->|Attach Bearer Token| ApiS
        CP -->|HTTP Requests| ApiS
        AP -->|HTTP Requests| ApiS
    end

    subgraph Backend [.NET 10 Web API]
        AC[Auth Controller]
        UC[User Controller]
        RC[Rooms Controller]
        RSC[Reservation Controller]
        
        AuthSvc[Auth Service]
        AR[Auth Repository]
        UR[User Repository]
        RR[Room Repository]
        RSR[Reservation Repository]

        ApiS -->|/api/auth/login| AC
        ApiS -->|/api/user| UC
        ApiS -->|/api/rooms| RC
        ApiS -->|/api/reservation| RSC

        AC --> AuthSvc
        AuthSvc --> AR
        UC --> UR
        RC --> RR
        RSC --> RSR
    end

    subgraph Database [MySQL Database]
        DB[(hotel_management_api_db)]
        AR -->|MySqlConnection| DB
        UR -->|MySqlConnection| DB
        RR -->|MySqlConnection| DB
        RSR -->|MySqlConnection| DB
    end
```

---

## 📊 Database Schema

The database relies on a relational schema with foreign key constraints connecting users, rooms, and reservations.

```mermaid
erDiagram
    USER {
        int ID PK
        string Kullanici_Adi UK
        string Sifre
        string Rol "DEFAULT: customer"
    }
    ROOM {
        int ID PK
        string Oda_Numarasi UK
        string Tip
        decimal Gecelik_Fiyat
        int Durum "0: Empty, 1: Reserved"
    }
    RESERVATION {
        int ID PK
        int Oda_ID FK
        string Musteri_Ad_Soyad FK
        date Giris_Tarihi
        date Cikis_Tarihi
        decimal Toplam_Ucret
    }
    USER ||--o{ RESERVATION : "makes"
    ROOM ||--o{ RESERVATION : "has many"
```

### SQL Script (`Hotel_Management_db.sql`)

```sql
CREATE SCHEMA hotel_management_api_db;
USE hotel_management_api_db;

CREATE TABLE room (
  ID INT AUTO_INCREMENT PRIMARY KEY,
  Oda_Numarasi VARCHAR(10) UNIQUE NOT NULL,
  Tip VARCHAR(50) NOT NULL,
  Gecelik_Fiyat DECIMAL(10,2) NOT NULL,
  Durum INT DEFAULT 0
);

CREATE TABLE user (
  ID INT AUTO_INCREMENT PRIMARY KEY,
  Kullanici_Adi VARCHAR(50) NOT NULL UNIQUE,
  Sifre VARCHAR(255) NOT NULL,
  Rol VARCHAR(20) DEFAULT 'customer'
);

CREATE TABLE reservation (
  ID INT AUTO_INCREMENT PRIMARY KEY,
  Oda_ID INT,
  Musteri_Ad_Soyad VARCHAR(100),
  Giris_Tarihi DATE NOT NULL,
  Cikis_Tarihi DATE NOT NULL,
  Toplam_Ucret DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (Oda_ID) REFERENCES room(ID),
  FOREIGN KEY (Musteri_Ad_Soyad) REFERENCES user(Kullanici_Adi)
);
```

---

## 📡 API Reference

### 🔑 Authentication & Users (`/api/auth`, `/api/user`)

| Method | Endpoint | Description | Body Params |
|---|---|---|---|
| `POST` | `/api/auth/login` | Authenticate user and issue JWT Token | `{ username, password }` |
| `POST` | `/api/user` | Register a new user account | `{ kullanici_Adi, sifre }` |

### 🚪 Rooms (`/api/rooms`)

| Method | Endpoint | Description | Query / Body Params |
|---|---|---|---|
| `GET` | `/api/rooms` | Fetch list of all rooms | - |
| `GET` | `/api/rooms/available` | Get available rooms for a given date range | `checkIn` (Date), `checkOut` (Date) |
| `POST` | `/api/rooms` | Create a new room | `{ oda_Numarasi, tip, gecelik_Fiyat }` |
| `PUT` | `/api/rooms/{id}` | Update room details | `{ oda_Numarasi, tip, gecelik_Fiyat, durum }` |
| `DELETE` | `/api/rooms/{id}` | Remove a room by ID | Path variable `id` |

### 📅 Reservations (`/api/reservation`)

| Method | Endpoint | Description | Query / Body Params |
|---|---|---|---|
| `GET` | `/api/reservation` | Retrieve all reservations with room details | - |
| `POST` | `/api/reservation` | Create a new room reservation | `{ oda_ID, musteri_Ad_Soyad, giris_Tarihi, cikis_Tarihi }` |
| `DELETE` | `/api/reservation/{id}` | Delete / Cancel a reservation | Path variable `id` |

---

## 📂 Project Directory Structure

```
Hotel_Management_System/
│
├── Hotel_Management_Api/                  # .NET 10 Web API Backend Project
│   └── Hotel_Management_Api/
│       ├── Controllers/                   # API Endpoints (AuthController, UserController, RoomsController, ReservationController)
│       ├── DTOs/                          # Data Transfer Objects (LoginRequestDto, LoginResponseDto, UserDto, RoomDto, etc.)
│       ├── Interfaces/                    # Repository & Service Interfaces (IAuthRepository, IAuthService, IUserRepository, etc.)
│       ├── Models/                        # Entity Domain Models (User, Room, Reservation)
│       ├── Repositories/                  # Data Access Layer using MySql.Data (AuthRepository, UserRepository, RoomRepository, etc.)
│       ├── Services/                      # Business & JWT Token Logic (AuthService)
│       ├── Program.cs                     # JWT Middleware, Dependency Injection & CORS Config
│       └── appsettings.json               # JWT Secret Key & Database Connection String
│
├── Hotel_Management_Frontend/             # Angular 21 Single-Page Application
│   └── src/app/
│       ├── components/                    # UI Components
│       │   ├── header/                    # Dynamic Navigation Header (Auth Status & Logout)
│       │   └── login/                     # Login & Account Registration Page
│       ├── guards/                        # Route Protection Guards (auth.guard.ts: authGuard, adminGuard, loginGuard)
│       ├── models/                        # TypeScript Interfaces (Room, Reservation, UserDto)
│       ├── pages/                         # Main Page Views
│       │   ├── admin/                     # Admin Dashboard (Room CRUD & Reservation Oversight)
│       │   └── customer/                  # Guest Reservation Portal
│       ├── services/                      # Services
│       │   ├── api.ts                     # REST Client Service
│       │   └── auth.ts                    # JWT Authentication & Role Management Service
│       ├── app.routes.ts                  # Application Routes & Guard Configurations
│       ├── app.config.ts                  # App Providers & HTTP Interceptor Registration
│       └── auth.interceptor.ts            # Bearer Token HTTP Interceptor
│
└── Hotel_Management_db.sql                # Database Creation & Table Schema Script
```

---

## ⚡ Getting Started & Setup Guide

### 1. Prerequisites
Ensure you have the following software installed:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+ recommended) and `npm`
- [Angular CLI](https://angular.dev/) (`npm install -g @angular/cli`)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) or MySQL Workbench / MariaDB

---

### 2. Database Setup
1. Open your MySQL client (e.g., MySQL Workbench or Command Line).
2. Execute the provided `Hotel_Management_db.sql` script to create the schema and required tables (`room`, `user`, `reservation`):
   ```bash
   mysql -u root -p < Hotel_Management_db.sql
   ```

---

### 3. Backend Setup (.NET Web API)
1. Navigate to the API folder:
   ```bash
   cd Hotel_Management_Api/Hotel_Management_Api
   ```
2. Configure your MySQL database connection string and JWT parameters in `appsettings.json` or `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=hotel_management_api_db;Uid=root;Pwd=YOUR_MYSQL_PASSWORD;"
     },
     "Jwt": {
       "Issuer": "http://localhost:5151",
       "Audience": "http://localhost:4200",
       "Key": "YOUR_SECRET_JWT_KEY_HERESHOULDBE32BYTESMIN"
     }
   }
   ```
3. Restore dependencies and run the API server:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Access the Swagger UI for interactive API documentation:
   - **Swagger URL:** `http://localhost:5151/swagger` (or `https://localhost:7028/swagger`)

---

### 4. Frontend Setup (Angular UI)
1. Navigate to the frontend workspace directory:
   ```bash
   cd Hotel_Management_Frontend
   ```
2. Install npm dependencies:
   ```bash
   npm install
   ```
3. Launch the Angular development server:
   ```bash
   npm start
   ```
4. Open your browser and navigate to:
   - **Login Page:** `http://localhost:4200/login`
   - **Customer Portal:** `http://localhost:4200/customer` (Requires authentication)
   - **Admin Dashboard:** `http://localhost:4200/admin` (Requires admin role)

---

## 🧪 Testing

### Frontend Unit Tests
Run unit tests powered by **Vitest**:
```bash
cd Hotel_Management_Frontend
npm run test
```

---

## 📝 License
This project is developed for educational and internship purposes. Feel free to fork, modify, and enhance it!
