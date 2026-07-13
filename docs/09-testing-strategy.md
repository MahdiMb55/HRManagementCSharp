# راهبرد آزمون

Domain.Tests الگوریتم‌ها و invariantهای خالص را بدون I/O می‌سنجد. Application.Tests workflow، filter semantics، cancellation، stale-result protection و typed failureها را روی fakeهای کوچک قراردادها می‌سنجد. Infrastructure.Tests با پوشه و SQLite موقت، mapping، index، query، migration، path، فایل و startup integrity را بررسی می‌کند.

هر رفتار جدید چرخه red-green-refactor دارد: آزمون متمرکز نوشته می‌شود، failure مورد انتظار دیده می‌شود، حداقل production code افزوده و سپس suite مرتبط اجرا می‌شود. آزمون timing-fragile مجاز نیست؛ `IClock`, `IDelay`, `IBackgroundExecutor`, `IApplicationPaths` و در صورت ارزش `IFileSystem` seam قطعی فراهم می‌کنند.

گیت نهایی شامل `dotnet format --verify-no-changes`, build Release، تمام testهای Release، script migration و اعمال migration روی مسیر موقت تازه است. آزمون infrastructure هرگز از Data واقعی استفاده نمی‌کند.
