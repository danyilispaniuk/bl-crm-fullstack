# BL CRM - Fullstack Contract Management System

A robust, enterprise-grade Contract Relationship Management (CRM) system built using **Clean Architecture** with a **.NET 10 Core Web API** backend and an **Angular 21** frontend. The entire application is fully dockerized with a multi-stage production setup, including SQL Server, ASP.NET Core, and Nginx with single-page application (SPA) routing.

---

## 🚀 Key Features

*   **Clean Architecture Bounded Contexts**: Decoupled core layers ensuring separation of concerns between domain entities, application logic, infrastructure persistence, and API controllers.
*   **Role-Based Access Control (RBAC)**: Secure routes and actions mapped to `Admin`, `Advisor`, and `Client` roles.
*   **Robust CSV Export Service**:
    *   **Excel-Friendly Formats**: Implemented formulas for phone numbers (`="+420..."`) to prevent leading-zero stripping and numerical corruption in Microsoft Excel and Google Sheets.
    *   **Fully-Deconstructed Contract Data**: Contracts export generates 12 structured columns with separate names, surnames, and IDs for Clients and Managers, plus a list of participating advisors.
    *   **Role-Restricted Endpoints**: Admin-only and Advisor-restricted API controllers.
*   **Modern Premium UI/UX**:
    *   Sleek dashboard with fluid animations and responsive glassmorphism styles.
    *   Role-aware dynamic headers with custom-designed action buttons (e.g. "+ New" and "Create .csv file").
    *   Beautiful micro-interactions (hover, active click scaling, and smooth color transitions).

---

## 🛠️ Technology Stack

### Backend
*   **Framework**: .NET 10 (Preview)
*   **ORM**: Entity Framework Core 10 (EF Core)
*   **Database**: Microsoft SQL Server
*   **Authentication**: JSON Web Token (JWT) with ASP.NET Core Identity
*   **CSV Processing**: CsvHelper
*   **Testing**: xUnit

### Frontend
*   **Framework**: Angular 21
*   **Styling**: Vanilla SCSS (Modern Flex/Grid layout, custom variables)
*   **State & HTTP**: RxJS observables & Angular Signals

### DevOps & Containers
*   **Docker Compose**: Multi-container orchestration (`db`, `backend`, `frontend`)
*   **Web Server**: Nginx (Alpine-based, customized for SPA redirection)

---

## ⚡ Getting Started (Docker Compose)

The easiest way to run the entire fullstack system (Database, Backend, and Frontend) is via Docker Compose:

1.  **Clone & Navigate to Repository Root**:
    ```bash
    cd bl-crm-fullstack
    ```
2.  **Start all Services**:
    ```bash
    docker compose up --build
    ```
3.  **Access the Application**:
    *   **Frontend**: Open [http://localhost:4200](http://localhost:4200) in your browser.
    *   **Backend API**: Access [http://localhost:5000/swagger](http://localhost:5000/swagger) (or `/api`) to test API controllers.
    *   **Database**: Connect via SQL Server Management Studio (SSMS) at `localhost,1433` using SA credentials.

---

## 💻 Manual Local Development

If you prefer to run the applications locally outside of Docker, follow these steps:

### 1. Database Setup
*   Ensure a local instance of SQL Server (such as LocalDB or SQLEXPRESS) is running.
*   Update the connection string under `backend/src/BL.CRM.API/appsettings.json` if needed.
*   Apply EF Core migrations & run the seeders:
    ```bash
    cd backend
    dotnet ef database update --project src/BL.CRM.Infrastructure --startup-project src/BL.CRM.API
    ```

### 2. Run the Backend API
```bash
cd backend
dotnet run --project src/BL.CRM.API/BL.CRM.API.csproj
```
The API will start listening on `http://localhost:5000` and `https://localhost:7089`.

### 3. Run Unit & Integration Tests
Run tests for application services and logic using the .NET Core test runner:
```bash
cd backend
dotnet test
```

### 4. Run the Frontend
1.  Navigate to the `frontend` directory:
    ```bash
    cd frontend
    ```
2.  Install dependencies:
    ```bash
    npm install
    ```
3.  Start the Angular development server:
    ```bash
    npm start
    ```
4.  Open [http://localhost:4200](http://localhost:4200) in your browser.

---

## 📖 User Guide

### 1. System Login

    Open the application in your browser at [http://localhost:4200](http://localhost:4200).

    To access the system with full administrator privileges, use the credentials provided in the Seed User Credentials section (admin@test.com, Password123!).

    Upon successful login, you will be automatically redirected to the main Dashboard.

### 2. Contract Management (CRUD)

    Viewing: Click on "Contracts" in the left navigation menu. You will see a tabular overview of all contracts. Clicking on a specific contract card will open its detailed view.

    Creation: In the contracts overview, click the "+ New Contract" button. Fill in the registration number, and select the institution, client, manager, and optionally participants.

    Entity Navigation: On the contract detail page, you can click directly on the client's manager's or participant's name, which will instantly redirect you to the detail page of that specific person.

    Editing: Click on the "Edit" button in the top right corner of the contract detail page. Fill in the updated information and click the "Update" button to save your changes.

    Deleting: Click on the "Delete" button in the top right corner of the contract detail page or click menu button in contract card in contracts list. You will be prompted to confirm the deletion. Click "Delete" to confirm the deletion.

### 3. Advisors Management (CRUD)

    Viewing: Click on "Advisors" in the left navigation menu. You will see a tabular overview of all advisors. Clicking on a specific advisor card will open its detailed view.

    Editing: Click on the "Edit" button in the top right corner of the advisor detail page. Fill in the updated information and click the "Update" button to save your changes.

    Deleting: Click on the "Delete" button in the top right corner of the advisor detail page or click menu button in advisor card in advisors list. You will be prompted to confirm the deletion. Click "Delete" to confirm the deletion.

    Creation: Advisor can create a new account by clicking a Sign up button in the login page and fill in the form with their details. When they click the "Sign Up" button, the system will automatically login with the new account.

### 4. Clients Management (CRUD)

    Viewing: Click on "Clients" in the left navigation menu. You will see a tabular overview of all clients. Clicking on a specific client card will open its detailed view.

    Creation: In the clients overview, click the "+ New Client" button. Fill in the first name, last name, and email address. You can also select a password for the new client, or the system will generate a random one.

    Editing: Click on the "Edit" button in the top right corner of the client detail page. Fill in the updated information and click the "Update" button to save your changes.

    Deleting: Click on the "Delete" button in the top right corner of the client detail page or click menu button in client card in clients list. You will be prompted to confirm the deletion. Click "Delete" to confirm the deletion.

### 5. CSV Data Export

    In the contracts (or clients/advisors) overview, simply click the "Export to CSV" button.

    The system will automatically generate and download a text file with structured data optimized for Microsoft Excel and Google Sheets (including protection against phone number formatting corruption).

## 🔐 Seed User Credentials

On first run, the database is automatically seeded with default accounts for all roles:

| Role | Email | Password | Details |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@test.com` | `Password123!` | Full admin access (Manage users, manage all contracts, export CSVs). |
| **Advisor** | `advisor1@test.com` to `advisor10@test.com` | `Password123!` | Advisor access (Manage own contracts and edit clients). |
| **Client** | `client1@test.com` to `client10@test.com` | *None* | No access |

---

## 📂 Project Structure

```text
bl-crm-fullstack/
├── backend/                       # ASP.NET Core Backend
│   ├── src/
│   │   ├── BL.CRM.Domain/         # Core Domain Entities
│   │   ├── BL.CRM.Application/    # DTOs, Services & Business Logic
│   │   ├── BL.CRM.Infrastructure/ # Database context, Seeders & Identity
│   │   ├── BL.CRM.API/            # Controllers & JWT Config
│   │   └── BL.CRM.Application.Tests/ # Unit & Integration Test suites (xUnit)
│   └── Dockerfile                 # Multi-stage Backend build
├── frontend/                      # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/        # Cards, navigation, overlays
│   │   │   ├── pages/             # Dashboard, client, advisor grids
│   │   │   └── services/          # HTTP request handlers & state
│   │   └── styles.scss            # Global styling system
│   ├── Dockerfile                 # Multi-stage Frontend build
│   └── nginx.conf                 # Nginx single-page-routing config
└── docker-compose.yml             # Local deployment orchestration config
```

## 🔮 Tech Debt & Future Enhancements

While the current implementation fulfills the core requirements and demonstrates a clean, maintainable architectural approach, a production-ready evolution of this system would include the following enhancements:

* **Scalability & Performance**:
    * **Pagination & Advanced Filtering**: Implement pagination and dynamic filtering on list endpoints to ensure consistent performance as the database grows to thousands of records.
    * **Distributed Caching**: Introduce **Redis** to cache dashboard statistics and frequently accessed, rarely changing data (like client lists) to reduce SQL Server load.
* **Observability & DevOps**:
    * **Structured Logging**: Integrate **Serilog** with sinks for Seq or Azure Application Insights to trace API requests, monitor performance bottlenecks, and centralize error logging.
    * **CI/CD Pipelines**: Set up GitHub Actions or Azure DevOps pipelines to automatically run the `xUnit` test suites and enforce code quality checks on every pull request.
* **Advanced Quality Assurance**:
    * **End-to-End (E2E) Testing**: Implement automated UI testing using **Playwright** or **Cypress** to verify critical user journeys (e.g., the complete contract creation flow).y
* **Enhanced UX & Real-Time Features**:
    * **SignalR Integration**: Add WebSockets (via ASP.NET Core SignalR) to push real-time notifications to connected Advisors when the Admin assigns a new contract to them, eliminating the need for manual page refreshes.
