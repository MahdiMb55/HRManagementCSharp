# HRManagement

سامانه آفلاین و تک‌کاربره مدیریت منابع انسانی برای ویندوز، با رابط فارسی و راست‌به‌چپ.

## فناوری و معماری

- .NET 10 و Windows Forms
- SQLite و Entity Framework Core 10
- معماری چهارلایه `Domain -> Application -> Infrastructure -> WinForms`
- الگوی Passive View / MVP در رابط کاربری
- xUnit برای آزمون‌های خودکار

## ساخت و آزمون

```powershell
dotnet restore HRManagement.sln
dotnet build HRManagement.sln
dotnet test HRManagement.sln
dotnet format HRManagement.sln --verify-no-changes
```

## انتشار و نصب

```powershell
scripts\Publish.ps1
scripts\Verify-Publish.ps1
```

پس از انتشار، نصب‌کننده Inno Setup از `installer\HRManagement.iss` ساخته می‌شود. خروجی نصب نباید پایگاه داده توسعه را همراه داشته باشد و پوشه `Data` باید در مقصد نصب حفظ شود.

داده عملیاتی در `Data` کنار فایل اجرایی قرار می‌گیرد. نصب‌کننده باید مقصدی قابل‌نوشتن انتخاب کند؛ برنامه به `AppData` یا `ProgramData` جابه‌جا نمی‌شود.

اسناد محصول و معماری از [docs/01-product-requirements.md](docs/01-product-requirements.md) آغاز می‌شوند.
