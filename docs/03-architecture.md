# معماری

## سبک

سامانه از معماری چهارلایه عمل‌گرا و MVP سبک استفاده می‌کند:

```text
HRManagement.WinForms -> HRManagement.Infrastructure -> HRManagement.Application -> HRManagement.Domain
HRManagement.WinForms -------------------------------> HRManagement.Application
```

Domain شامل موجودیت، enum، value object و قواعد خالص است. Application سناریوها، قرارداد مخزن/فایل/زمان/اجرای پس‌زمینه و DTOها را هماهنگ می‌کند. Infrastructure نگاشت EF Core، SQLite، فایل، پشتیبان، logging و اجرای قراردادها را دارد. WinForms فقط نمایش، presenter، ناوبری، تبدیل جلالی و bootstrap را نگه می‌دارد.

## مرزهای اجرا

هر عملیات پایگاه داده یک DbContext تازه از `IDbContextFactory<HrManagementDbContext>` می‌گیرد. queryهای فهرست `AsNoTracking`، projected و paginated هستند. presenter شناسه درخواست جاری و `CancellationTokenSource` را نگه می‌دارد تا نتیجه قدیمی روی نتیجه جدید ننشیند. عملیات بالقوه مسدودکننده SQLite از `IBackgroundExecutor` عبور می‌کند و تنها update نهایی UI به thread رابط بازمی‌گردد.

## راه‌اندازی

`Program` فقط تنظیم WinForms، گرفتن mutex و اجرای `ApplicationBootstrapper` را انجام می‌دهد. bootstrapper مسیرها، marker، قابلیت نوشتن، logging، DI، integrity، migration، seed و font را آماده می‌کند و سپس `MainForm` را می‌سازد. mutex ثابت `Local\\HRManagement.SingleInstance.v1` است.

## خطا

خطاهای قابل انتظار به نتیجه typed با کد و پیام فارسی تبدیل می‌شوند. exception فنی با operation ID ثبت می‌شود. UI stack trace، مسیر داده، کد ملی کامل، کارت، IBAN یا محتوای سند را نشان نمی‌دهد.
