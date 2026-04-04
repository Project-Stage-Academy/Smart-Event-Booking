# 🎟 Smart Event Booking System

## 📌 Project Vision Statement (PVS)

Smart Event Booking System is a web application designed to simplify the process of discovering, managing, and booking events such as concerts, lectures, and webinars.

The system enables users to easily browse events, reserve tickets, and track their booking history, while administrators can efficiently create and manage events.

The goal of the project is to provide a user-friendly, scalable, and reliable platform that demonstrates modern web development practices using ASP.NET MVC, including authentication, data management, and clean architecture principles.

---

## 🤝 Contribution Flow

To ensure smooth collaboration during the internship, follow this workflow:

### 1. Branching Strategy
- `main` – stable production-ready code
- `develop` – integration branch
- `feature/*` – new features
- `bugfix/*` – bug fixes

### 2. Workflow Steps
1. Pull latest changes from `develop`
2. Create a new branch:
```

git checkout -b feature/your-feature-name

```
3. Implement your task
4. Commit changes with meaningful messages:
```

feat: add event creation form
fix: resolve booking validation issue

```
5. Push branch to remote
6. Create Pull Request (PR) to `develop`
7. Request code review
8. Fix comments if needed
9. Merge after approval

### 3. Code Requirements
- Follow clean architecture principles
- Use meaningful naming conventions
- Apply validation where needed
- Write readable and maintainable code

---

## 📦 Project Scope

The project is divided into epics and user stories.

---

### 🔹 EPIC 1: User Authentication & Authorization

**Goal:** Enable secure user registration and login.

**User Stories:**
- As a user, I want to register an account so that I can use the system
- As a user, I want to log in so that I can access my profile
- As a user, I want to log out securely
- As a system, I want to differentiate between Admin and User roles

---

### 🔹 EPIC 2: Event Management (Admin)

**Goal:** Allow admins to create and manage events.

**User Stories:**
- As an admin, I want to create a new event
- As an admin, I want to edit event details
- As an admin, I want to delete events
- As an admin, I want to view all created events

---

### 🔹 EPIC 3: Event Browsing (User)

**Goal:** Allow users to explore available events.

**User Stories:**
- As a user, I want to view a list of events
- As a user, I want to view event details
- As a user, I want to filter or search events

---

### 🔹 EPIC 4: Ticket Booking

**Goal:** Enable users to book tickets for events.

**User Stories:**
- As a user, I want to book a ticket for an event
- As a user, I want to see available seats or capacity
- As a system, I want to prevent overbooking
- As a user, I want to cancel my booking

---

### 🔹 EPIC 5: Booking History

**Goal:** Provide users with access to their booking history.

**User Stories:**
- As a user, I want to view my past bookings
- As a user, I want to see booking details
- As a user, I want to track my active reservations

---

### 🔹 EPIC 6: QR Ticket Generation

**Goal:** Provide a simple digital ticket.

**User Stories:**
- As a user, I want to receive a QR code after booking
- As a system, I want to generate a unique QR per booking
- As a user, I want to view my QR ticket

---

## 🚀 Tech Stack (recommended)

- ASP.NET MVC / ASP.NET Core
- Entity Framework Core
- MS SQL Server
- ASP.NET Identity
- Optional: JavaScript / React (frontend enhancement)

---

## 📌 Notes

- Focus on delivering a working MVP within 1 month
- Prioritize functionality over complex UI
- Each feature should be demo-ready

---

## 🎯 Final Deliverable

A working web application that demonstrates:
- Authentication & authorization
- CRUD operations
- Booking logic
- Clean architecture
- Team collaboration
