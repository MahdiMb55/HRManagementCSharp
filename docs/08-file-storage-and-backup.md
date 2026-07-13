# ذخیره فایل و پشتیبان

## چیدمان

ریشه از `AppContext.BaseDirectory` resolve می‌شود و `Data/Database`, `Documents`, `Photos`, `LetterTemplates`, `GeneratedLetters`, `Backups`, `Trash`, `Logs` و `Temp` زیر آن قرار می‌گیرند. پایگاه داده `Data/Database/hr-management.db` است. marker نصب، اجرای نخست را از مفقودشدن پایگاه داده پس از استفاده جدا می‌کند. fallback به AppData یا ProgramData وجود ندارد.

فایل مجاز PDF، تصویر رایج، Word و Excel تا 20 MiB است. extension و signature محتوایی بررسی، نام داخلی تصادفی تولید، hash محاسبه و copy اتمیک می‌شود. نام اصلی فقط metadata است؛ مسیر ذخیره نسبی و canonical است و traversal رد می‌شود. فایل مبدأ کاربر حذف نمی‌شود. حذف ابتدا فایل را به Trash داخلی منتقل می‌کند.

## پشتیبان و بازیابی

پشتیبان شامل snapshot سازگار SQLite با WAL، اسناد، تصاویر، logo، templateها، خروجی نامه و تنظیمات است. manifest شامل نسخه schema، فهرست مسیر، اندازه و hash است. archive نهایی atomically در مقصد دستی/خودکار ساخته می‌شود. پیش از restore پشتیبان اضطراری گرفته و ساختار کامل اعتبارسنجی می‌شود؛ پایگاه داده و فایل‌ها فقط به‌صورت یک واحد جایگزین می‌شوند و شکست restore مسیر rollback دارد. فایل‌های پشتیبان حاوی داده حساس و در نسخه یک رمزنگاری‌نشده‌اند.
