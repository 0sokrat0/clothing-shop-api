# ClothingShop API

API магазина одежды (в первую очередь футболок). ASP.NET Core 9, Minimal API, EF Core + SQLite, JWT.

## Архитектура

Clean Architecture по мотивам [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
и [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture). Зависимости направлены только внутрь:

```
Api ──► Infrastructure ──► Application ──► Domain
 └──────────────────────────►┘
```

| Проект | Что лежит | От чего зависит |
|---|---|---|
| `ClothingShop.Domain` | Сущности (`Clothing`, `ClothingSize`, `Order`, `OrderItem`, `User`), енумы, правило резерва товара `ClothingSize.Reserve` | ни от чего |
| `ClothingShop.Application` | Сценарии (`AuthService`, `ClothingService`, `OrderService`), DTO, интерфейсы `IAppDbContext`, `IPasswordHasher`, `ITokenService` | Domain |
| `ClothingShop.Infrastructure` | `AppDbContext` (EF Core), сидинг БД, PBKDF2-хеширование паролей, выпуск JWT | Application |
| `ClothingShop.Api` | Эндпоинты, авторизация, перевод исключений в HTTP-коды | Application, Infrastructure |

Слой Application не знает ни про SQLite, ни про JWT: он работает с интерфейсами, реализации подставляет Infrastructure через DI.

## База данных

```
Users 1──* Orders 1──* OrderItems *──1 ClothingSizes *──1 Clothes
```

* `Clothes`: название, цена, описание, количество покупок, тип, доступность.
* `ClothingSizes`: буква (`S`, `M`, `L`, `XL`), число, остаток на складе.
* `Orders`: дата создания, статус (`New`, `Processing`, `Shipped`, `Delivered`, `Cancelled`), покупатель.
* `OrderItems`: какой размер какого товара и сколько штук.
* `Users`: логин, хеш пароля (PBKDF2-SHA256 с солью), телефон, адрес, роль (`Customer`, `Manager`).

БД создаётся при первом запуске (`EnsureCreated`) и заполняется: 4 товара по 4 размера и менеджер `manager` / `manager123`.

## Запуск

```bash
dotnet run --project ClothingShop.Api
```

API поднимается на `http://localhost:5080`.

## Эндпоинты

| Метод | Путь | Доступ | Что делает |
|---|---|---|---|
| POST | `/api/auth/register` | все | регистрация покупателя |
| POST | `/api/auth/login` | все | вход, возвращает JWT |
| GET | `/api/clothes` | все | вся одежда |
| GET | `/api/clothes?search=shirt` | все | поиск по части названия |
| POST | `/api/orders` | авторизованный | создать заказ |
| GET | `/api/orders?status=New` | авторизованный | заказы со статусом (покупатель видит свои, менеджер все) |
| GET | `/api/users/{id}/orders` | авторизованный | заказы покупателя (покупатель только свои) |
| PATCH | `/api/orders/{id}/status` | менеджер | сменить статус заказа |
| PATCH | `/api/sizes/{id}/quantity` | менеджер | изменить остаток размера |
| POST | `/api/clothes/{id}/remove-from-sale` | менеджер | убрать из продажи |
| POST | `/api/clothes/{id}/return-to-sale` | менеджер | вернуть в продажу |

Ошибки приходят как `{ "error": "..." }` с кодами 400 / 401 / 403 / 404.

## Postman

Импортировать `ClothingShop.postman_collection.json` и запускать запросы сверху вниз (или Run collection).
Запросы логина сами сохраняют токены в переменные `token` и `managerToken`, создание заказа сохраняет `orderId`.

## Распределение задач и ветки

| Участник | Ветка | Задачи |
|---|---|---|
| Участник 1 | `feature/domain-db` | Domain, `AppDbContext`, сидинг |
| Участник 1 | `feature/clothes` | `ClothingService`, эндпоинты каталога и склада |
| Участник 2 | `feature/auth` | хеширование паролей, JWT, регистрация и логин, роли |
| Участник 2 | `feature/orders` | `OrderService`, эндпоинты заказов, коллекция Postman |

Каждая ветка вливается в `main` через Pull Request.
