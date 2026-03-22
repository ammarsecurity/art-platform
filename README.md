# 🎨 منصة الفن — Art Platform

منصة عربية احترافية للفنون البصرية والتعليم الإبداعي.

---

## 🗂️ هيكل المشروع

```
art/
├── backend/                     # .NET Core 10 — Clean Architecture
│   ├── ArtPlatform.sln
│   ├── ArtPlatform.Domain/      # Entities, Enums
│   ├── ArtPlatform.Application/ # DTOs, Interfaces, Services (contracts)
│   ├── ArtPlatform.Infrastructure/ # EF Core, Repositories, Services (impl)
│   └── ArtPlatform.API/         # Controllers, Middleware, Program.cs
├── frontend/                    # Vue 3 + Tailwind CSS
│   ├── src/
│   │   ├── assets/              # CSS global
│   │   ├── components/
│   │   │   ├── layout/          # Navbar, Footer, AdminLayout
│   │   │   └── ui/              # ArtworkCard, CourseCard, Pagination
│   │   ├── views/
│   │   │   ├── public/          # Home, Portfolio, Courses, Blog, Contact, About
│   │   │   ├── auth/            # Login, Register
│   │   │   ├── admin/           # Dashboard, Artworks, Courses, Blog, Categories, Messages
│   │   │   └── courses/         # LearnView (video player)
│   │   ├── stores/              # Pinia: auth, artworks, courses
│   │   ├── services/            # Axios API service
│   │   └── router/              # Vue Router with guards
│   └── package.json
└── database/
    └── schema.sql               # MySQL schema + seed data
```

---

## 🚀 إعداد المشروع

### المتطلبات
- .NET 10 SDK
- Node.js 20+
- MySQL 8.0+

### 1. إعداد قاعدة البيانات

```bash
mysql -u root -p < database/schema.sql
```

### 2. إعداد الـ Backend

```bash
cd backend

# تعديل connection string في appsettings.json
# "DefaultConnection": "Server=localhost;Database=art_platform;User=root;Password=YOUR_PASS;..."

# استعادة الحزم وتشغيل migration
cd ArtPlatform.API
dotnet restore ../ArtPlatform.sln
dotnet ef migrations add InitialCreate --project ../ArtPlatform.Infrastructure --startup-project .
dotnet ef database update --project ../ArtPlatform.Infrastructure --startup-project .

# تشغيل الخادم
dotnet run
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### 3. إعداد الـ Frontend

```bash
cd frontend
npm install

# تشغيل في وضع التطوير
npm run dev
# http://localhost:5173

# بناء للإنتاج
npm run build
```

---

## 🔑 بيانات الدخول الافتراضية

بعد تشغيل المشروع، أنشئ مستخدم Admin عبر:

```bash
# تعديل hash كلمة المرور في قاعدة البيانات
# أو استخدام endpoint التسجيل ثم تحديث الدور يدوياً
```

أو استخدم endpoint التسجيل:
```
POST /api/auth/register
{
  "name": "مدير النظام",
  "email": "admin@artplatform.com",
  "password": "Admin@1234"
}
```
ثم حدّث الـ role في قاعدة البيانات:
```sql
UPDATE users SET role = 'Admin' WHERE email = 'admin@artplatform.com';
```

---

## 📡 API Endpoints

### Auth
| Method | Endpoint          | Description       | Auth |
|--------|-------------------|-------------------|------|
| POST   | /api/auth/login   | تسجيل الدخول      | —    |
| POST   | /api/auth/register| إنشاء حساب         | —    |
| GET    | /api/auth/me      | بيانات المستخدم   | JWT  |

### Artworks
| Method | Endpoint              | Description           | Auth       |
|--------|-----------------------|-----------------------|------------|
| GET    | /api/artworks         | قائمة الأعمال          | —          |
| GET    | /api/artworks/featured| الأعمال المميزة        | —          |
| GET    | /api/artworks/{slug}  | تفاصيل عمل            | —          |
| POST   | /api/artworks         | إضافة عمل             | Admin      |
| PUT    | /api/artworks/{id}    | تعديل عمل             | Admin      |
| DELETE | /api/artworks/{id}    | حذف عمل               | Admin      |

### Courses
| Method | Endpoint                      | Description         | Auth    |
|--------|-------------------------------|---------------------|---------|
| GET    | /api/courses                  | قائمة الدورات        | —       |
| GET    | /api/courses/featured         | الدورات المميزة      | —       |
| GET    | /api/courses/{slug}           | تفاصيل دورة         | —       |
| POST   | /api/courses                  | إضافة دورة          | Admin   |
| PUT    | /api/courses/{id}             | تعديل دورة          | Admin   |
| DELETE | /api/courses/{id}             | حذف دورة            | Admin   |
| POST   | /api/courses/{id}/enroll      | تسجيل في دورة       | Student |
| POST   | /api/courses/progress         | تحديث التقدم         | Student |
| POST   | /api/courses/lessons          | إضافة درس           | Admin   |
| DELETE | /api/courses/lessons/{id}     | حذف درس             | Admin   |

### Blog & Contact & Categories
| Method | Endpoint             | Description    |
|--------|----------------------|----------------|
| GET    | /api/blog            | المقالات        |
| GET    | /api/blog/{slug}     | تفاصيل مقال    |
| POST   | /api/contact         | إرسال رسالة     |
| GET    | /api/categories      | التصنيفات       |
| GET    | /api/admin/dashboard | إحصائيات Admin  |

---

## 🎨 ميزات التصميم

- **الثيم**: داكن (`#1E1E1E`) + ذهبي (`#C9A96E`)
- **الخط**: Cairo / Tajawal
- **RTL**: دعم كامل للعربية
- **Responsive**: Mobile-first
- **Animations**: Fade-in, Slide-up, Float

---

## 🏗️ Architecture

```
Clean Architecture:
┌─────────────────────────────────┐
│  API Layer (Controllers)        │  ← HTTP, Validation, Auth
├─────────────────────────────────┤
│  Application Layer (Services)   │  ← Business Logic, DTOs
├─────────────────────────────────┤
│  Domain Layer (Entities)        │  ← Core Models, Enums
├─────────────────────────────────┤
│  Infrastructure (Repos, DB)     │  ← MySQL, EF Core, File Storage
└─────────────────────────────────┘
```

---

## 📦 الحزم الرئيسية

### Backend
- **Pomelo.EntityFrameworkCore.MySql** — MySQL + EF Core
- **BCrypt.Net-Next** — تشفير كلمات المرور
- **System.IdentityModel.Tokens.Jwt** — JWT Authentication
- **Swashbuckle.AspNetCore** — Swagger UI

### Frontend
- **Vue 3** + Composition API
- **Pinia** — State Management
- **Vue Router 4** — Routing + Guards
- **Tailwind CSS** — Utility CSS
- **Axios** — HTTP Client
- **vue3-toastify** — Toast Notifications

---

## 🔒 الأمان

- JWT Bearer Authentication
- BCrypt password hashing
- CORS configuration
- Role-based authorization (Admin / Student)
- Input validation (DataAnnotations)
- Global exception handler middleware
- File upload validation (type + size)

---

## 🚀 Deploy للإنتاج

```bash
# Backend
dotnet publish -c Release -o ./publish

# Frontend
npm run build
# dist/ folder → serve with Nginx
```

**Nginx config للـ SPA:**
```nginx
location / {
  try_files $uri $uri/ /index.html;
}
```
