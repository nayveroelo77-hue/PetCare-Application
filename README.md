# PetCare - Modern .NET MAUI Application

PetCare is a professional, student-focused .NET MAUI application designed to manage pet care services with a clean, modern, and cross-platform experience. The app features a robust authentication system, role-based navigation, and persistent data storage.

---

## 🚀 Key Features

### 🔐 Authentication & Security
- **SQLite Persistence**: Integrated local storage using `sqlite-net-pcl` for user accounts and session data.
- **Persistent Session**: Users remain logged in across app restarts, with the app automatically navigating to the correct dashboard upon launch.
- **Navigation Protection**: Utilizes .NET MAUI Shell absolute routing (`//`) to clear the navigation stack after login, preventing unauthorized "Back" navigation.
- **Password Toggles**: Professional show/hide password functionality with stateful eye icons.

### 🏛️ Role-Based Architecture & Capabilities

**Admin Capabilities**:
- **Dashboard Overview**: View global statistics like total users, pets, and appointments.
- **Manage Users**: View and manage all client accounts registered in the system.
- **Manage Pets**: Add, edit, view, and oversee all registered pets.
- **Manage Appointments & Clinical Records**: Review, approve, and complete scheduled appointments across the clinic. Administrators can add medical treatment notes to finalized visits, which instantly update the client's pet health records.

**Client Capabilities**:
- **Personal Dashboard**: Quick overview of their registered pets and upcoming visits.
- **My Pets & Health Records**: View detailed profiles and clinical history for their own pets. Treatment notes logged by the admin from completed appointments are automatically visible to the client.
- **Educational Care Guides**: Access a library of expert guides on pet grooming, nutrition, health, and general maintenance (e.g., hair trimming, nail care, and first-aid).
- **Book & Manage Appointments**: Schedule new clinic visits and review their upcoming or past appointments.
- **Profile Management**: Update their personal user information.

**Automated Seeding**:
- Automatic creation of a default administrator account on first run.

### 🔄 Application Flow
1. **Startup & Authentication**: The app launches and checks for an existing session. Unauthenticated users are directed to the **Login** or **Register** Page.
2. **Role Authorization**: Upon successful login, the system verifies whether the user account has `Admin` or `Client` privileges.
3. **Protected Navigation**: The application uses absolute routing (`//AdminDashboard` or `//ClientDashboard`) to transition the user to their designated portal. This clears the navigation history, ensuring users cannot press the "Back" button to return to the login screen.
4. **Role-Specific UI**: Once logged in, the user interacts with an `AdminShell` or `ClientShell`. These custom navigation shells provide tabbed interfaces tailored explicitly to the user's permissions.

### ⚕️ Clinical Workflow & Treatment Records
1. **Booking**: Clients schedule appointments securely from their dashboard. The appointment is created with a `Pending/Scheduled` status.
2. **Approval**: Administrators review pending appointments via their Management portal and approve them for the clinic.
3. **Execution & Treatment**: Following the physical clinic visit, an Administrator clicks the "Complete & Add Notes" action button on the approved appointment. This prompts them to insert specific medical observations, prescriptions, or a diagnosis.
4. **Health Record Sync**: The appointment is marked as `Completed`. The inputted treatment notes are permanently stored and formatted into a Clinical History timeline, which is instantly and exclusively visible to the pet's owner.

---

## 📖 User Guides

### 👨‍💼 Administrator Tutorial: Adding Treatment Notes
Properly documenting pet care is essential for maintaining a professional clinic history. Follow these steps:
1. **Login** using the Admin credentials (`admin@petcare.com` / `admin123`).
2. Navigate to the **Appointments** tab in the sidebar.
3. Locate an appointment that is currently in the **Approved** status.
4. Click the **📝 (Complete & Add Notes)** icon on the right side of the appointment card.
5. In the pop-up prompt, type your professional observations, medications prescribed, or aftercare instructions.
6. Click **Complete**. The record is now finalized and instantly shared with the pet owner.

### 🏠 Client Tutorial: Accessing Care Guides & Records
Getting the best care for your pet is simple and informative:
- **Viewing Health History**: Go to **My Pets**, select your pet, and tap **View Health Records**. You will see a chronological timeline of every visit, including the exact notes left by the veterinarian.
- **Learning with Care Guides**: Tap the **Guides** icon in the bottom navigation bar. You can browse expert advice on:
    - **Grooming**: How to safely trim hair and nails at home.
    - **Safety**: A list of common household foods that are toxic to pets.
    - **Emergency**: Basic First Aid and pet CPR instructions.
- **Booking a Visit**: Tap **Visits** -> **Book Appointment**. Choose your pet, the service type, and the date/time. Your request will then wait for the administrator's approval.

---

### 🎨 Premium Design System
- **Responsive Layouts**: 2-column grid layouts for Windows/Desktop and a single-column stacked layout for mobile.
- **Modern Aesthetics**: Vibrant blue/cyan gradients, glassmorphism card styles, and professional Poppins typography.
- **Centralized Styling**: All UI components (Buttons, Borders, Inputs) are defined in `Styles.xaml` for consistency and maintainability.

---

## 🛠️ Technology Stack
- **Framework**: .NET MAUI (.NET 10)
- **Architecture**: MVVM Pattern (via `CommunityToolkit.Mvvm`)
- **Database**: Local SQLite (`sqlite-net-pcl`)
- **UI Toolkit**: `CommunityToolkit.Maui`
- **Fonts**: Poppins (Regular, Bold), OpenSans (Regular, Semibold)

---

## 🗝️ Default Admin Credentials
For initial setup and testing, a default administrator is seeded automatically:
- **Email**: `admin@petcare.com`
- **Password**: `admin123`

---

## 📁 Project Structure Highlights
- `/Model`: Data entities (e.g., `UserAccount.cs`).
- `/Service`: Business logic helpers (`DatabaseService.cs`, `AuthService.cs`).
- `/ViewModel`: UI logic and data binding (Login, Register, Dashboards).
- `/Page`: XAML views organized by role (Admin, Client).
- `/Resources/Styles`: Centralized design system and theme tokens.

---

## 🔧 Getting Started
1. **Restore Dependencies**: Ensure all NuGet packages are restored.
2. **Build**: Run `dotnet build`.
3. **Launch**: Deploy to Windows, Android, or iOS. The SQLite database will initialize automatically during the first login or registration attempt.

> [!TIP]
> The database file `PetCare.db3` is stored in the application's local data directory (`FileSystem.AppDataDirectory`).

> [!IMPORTANT]
> This application uses **Shell Hierarchical Routing**. Absolute routes like `//AdminDashboard` are used for root state transitions to ensure a clean navigation history.
