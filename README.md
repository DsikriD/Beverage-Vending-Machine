# 🥤 Beverage Vending Machine

Веб-приложение для управления автоматом по продаже напитков, реализованное с использованием ASP.NET Core и Next.js.

**ВАЖНО**: Перед запуском backend необходимо настроить конфигурацию:

### 1. Строка подключения к базе данных:
```bash
cd backend-part
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=BeverageVendingMachine;Username=postgres;Password=ВАШ_ПАРОЛЬ"
```

### 2. CORS политики (опционально, есть значения по умолчанию):
```bash
cd backend-part
dotnet user-secrets set "Cors:AllowedOrigins" "http://localhost:3000,https://localhost:3000,http://127.0.0.1:3000,https://127.0.0.1:3000"
dotnet user-secrets set "Cors:AllowedMethods" "GET,POST,PUT,DELETE,OPTIONS"
dotnet user-secrets set "Cors:AllowedHeaders" "*"
```

### 3. Настройка Frontend:
Скопируйте файл `frontend-part/env.example` в `frontend-part/.env.local` и при необходимости измените URL API:

```bash
cd frontend-part
copy env.example .env.local
```

#### Переменные окружения (.env.local):

- **NEXT_PUBLIC_API_URL** - URL backend API (по умолчанию: `https://localhost:7280`)
- **NEXT_PUBLIC_DEV_MODE** - Режим разработки (по умолчанию: `true`)

**Важно**: 
- Файл `.env.local` должен быть создан в папке `frontend-part/`
- Переменные с префиксом `NEXT_PUBLIC_` доступны в браузере
- Если backend запущен на другом порту, измените `NEXT_PUBLIC_API_URL` соответственно
- Файл `.env.local` добавляется в `.gitignore` и не должен попадать в репозиторий

### 4. Настройка базы данных PostgreSQL:

Убедитесь, что PostgreSQL запущен и создайте базу данных:

```sql
CREATE DATABASE "BeverageVendingMachine";
```

Замените `ВАШ_ПАРОЛЬ` на ваш реальный пароль PostgreSQL.

### Запуск приложения

#### 1. Запуск Backend (ASP.NET Core):
```bash
cd backend-part
dotnet restore
dotnet ef database update
dotnet run --urls="https://localhost:7280"
```

#### 2. Запуск Frontend (Next.js):
```bash
cd frontend-part
npm install
npm run dev
```

#### 3. Доступ к приложению:
- **Frontend**: http://localhost:3000
- **Backend API**: https://localhost:7280
- **Swagger UI**: https://localhost:7280/swagger

### Структура проекта

```
BeverageVendingMachine/
├── backend-part/           # ASP.NET Core API
│   ├── Controllers/        # API контроллеры
│   ├── Models/            # Модели данных и DTOs
│   ├── Data/              # Entity Framework контекст
│   ├── Repositories/      # Репозитории
│   ├── Services/          # Бизнес-логика
│   └── Migrations/        # Миграции базы данных
└── frontend-part/         # Next.js приложение
    ├── src/
    │   ├── app/           # Страницы приложения
    │   ├── shared/        # Общие компоненты и утилиты
    │   └── store/         # Redux store
    └── public/            # Статические файлы
```

### Основные функции

- 🛒 **Каталог товаров** с фильтрацией и поиском
- 🛍️ **Корзина покупок** с управлением количеством
- 💰 **Оплата монетами** с автоматическим расчетом сдачи
- 📊 **Управление товарами** через импорт Excel/CSV
- 🔒 **Блокировка автомата** через SignalR
- 📱 **Адаптивный дизайн** для мобильных устройств

### Технологии

**Backend:**
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- SignalR
- Swagger/OpenAPI

**Frontend:**
- Next.js 15
- React 18
- TypeScript
- Redux Toolkit
- SCSS Modules

### Разработка

Для разработки рекомендуется использовать:
- **IDE**: Visual Studio 2022 или VS Code
- **База данных**: pgAdmin для управления PostgreSQL
- **API тестирование**: Swagger UI или Postman
