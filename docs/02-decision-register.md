# Decision Register

Later decisions override earlier conflicting assumptions. These 120 decisions are approved and final.

1. Employee responsibilities must be stored as dated history records.
2. Each employee may have multiple education records.
3. Each employee may have multiple bank accounts, with one primary salary account.
4. National code is ultimately mandatory and globally unique; later decisions override the earlier optional possibility.
5. Personnel number is manually entered by the user and must be unique.
6. Employment statuses include active, long-term leave, suspended, resigned, dismissed, retired, and deceased.
7. Dependents include relationship, education status, and insurance status in addition to identity information.
8. Document categories have defaults, and users may create additional categories.
9. Employee deletion is soft deletion with restoration support.
10. Payroll, salary, benefits, and deductions are excluded from version one.
11. “Job title” and “responsibility” are not separate concepts; only responsibility is used.
12. Job-title history is therefore merged into responsibility history.
13. Departments are hierarchical and may have parent departments.
14. Store mobile number, landline number, emergency-contact information, and email separately.
15. Home address is stored as one complete text value.
16. Military-service status uses predefined options.
17. Store the employee’s social-security insurance number.
18. Each employee has one primary profile photograph.
19. Access cards have history; only one card is active for an employee at a time.
20. A contract contains contract number, type, start date, optional end date, and notes.
21. Contract types are fixed: official, contractual, peymani, probationary, temporary, and project-based.
22. Permanent contracts may have no end date.
23. Employment termination is an independent record with type, date, reason, and notes.
24. A returning employee reuses the existing employee file and receives a new employment period.
25. Contracts may have Word, PDF, and image attachments.
26. Previous document versions are retained and one version is marked current.
27. Employee documents do not need issue and expiry dates in version one.
28. No expiry reminder feature is required in version one.
29. Support both manual and automatic backups.
30. The user can choose the backup destination.
31. Audit important changes, not every property edit.
32. Changing a personnel number requires a specific operation and a recorded reason.
33. Validate Iranian national codes using their check-digit algorithm.
34. The final rule is that employees cannot be registered without a national code.
35. Detect duplicates by personnel number and national code; warn about similar name and birth-date combinations.
36. Search supports name, surname, personnel number, national code, phone, department, responsibility, and employment status.
37. Export the employee list and filtered results to Excel.
38. Provide a printable and PDF-capable employee-summary report.
39. Future Word templates use simple placeholders such as `{{FirstName}}`.
40. Issued-letter history stores letter number, issue date, template, employee, and output file, but not a full employee-data snapshot.
41. Excel import includes validation, preview, and duplicate/error reporting before insertion.
42. Version one edits one employee at a time; no bulk editing.
43. The home screen is a lightweight dashboard.
44. The employee file uses organized tabs.
45. Initial employee creation uses essential information; other data can be completed later.
46. Required initial fields are first name, last name, personnel number, and valid national code.
47. Forms save only when the user presses Save.
48. Warn before closing or navigating away from unsaved changes.
49. Automatic-backup retention is configurable, such as retaining the latest 30 backups.
50. Before restore, create an emergency backup of the current state.
51. Gender contains only male and female.
52. Marital status contains single, married, divorced, and widowed.
53. Blood type is optional and selected from standard blood groups.
54. Military-service fields are displayed only for male employees.
55. Dependent relationships use fixed values such as spouse, child, father, mother, and other.
56. Education degrees use fixed values.
57. A used department cannot be deleted; it can only be deactivated.
58. Responsibilities come from a manageable responsibility list.
59. Card number and IBAN are globally unique.
60. Profile photos are resized and optimized, and only the optimized copy is retained.
61. The application manages one company.
62. Company branches and service locations are excluded.
63. Branch-transfer history is therefore excluded.
64. Company settings store company name, logo, national identifier, registration number, phone, and address.
65. Duplicate Excel-import rows are rejected and reported; they do not update existing employees.
66. Excel import accepts the standard template produced by the application.
67. Deleted managed files first move to the application’s internal trash.
68. Soft-deleted records remain in an archive and may be restored or permanently deleted.
69. On application close, create an automatic backup if no backup was created that day.
70. Backup should execute in the background while preserving UI responsiveness as much as safely possible.
71. The Inno Setup installer allows the user to choose a writable installation directory. Application data is stored under a `Data` directory next to the executable.
72. The application-data location is fixed after installation in version one.
73. Only one application instance may run at a time.
74. Show understandable errors to users and write technical details to logs.
75. Delete logs older than 30 days automatically.
76. General settings are stored in SQLite.
77. The interface is Persian-only and right-to-left.
78. Bundle and privately load the Vazirmatn font.
79. Version one has one lightweight light theme.
80. The main window uses a fixed side navigation menu and displays pages inside the main window.
81. Use Entity Framework Core.
82. Use EF Core migrations for schema evolution.
83. Internal record identifiers use `long`.
84. Employee details open in a separate window.
85. Only one employee-details window exists at a time.
86. Adding and editing use the same employee form structure.
87. The employee list uses a read-only `DataGridView` with pagination.
88. The user can select 25, 50, or 100 records per page.
89. Automatic search uses approximately 300 milliseconds of debounce.
90. Sorting is performed by SQLite, not only on the currently loaded page.
91. Reuse the existing employee-details window when a different employee is selected.
92. The employee-details window is non-modal.
93. Employee-detail tabs load their contents lazily on first use.
94. Do not display employee photos in the main table.
95. Selecting a row displays a lightweight summary panel with photo, name, responsibility, and department.
96. The main employee table is read-only; editing occurs in the employee form.
97. Support shortcuts such as `Ctrl+N`, `Ctrl+F`, and `Enter`.
98. Provide an advanced filter builder.
99. Search filters and sorting are cleared when the application closes.
100. Users may show, hide, and reorder supported employee-list columns.
101. Users are not exposed to raw AND/OR logic. Multiple selected values within one field use OR; different fields are combined with AND.
102. Date filters support exact date, from/to range, before, and after.
103. Persian search normalizes spaces, half-spaces, Arabic/Persian `ی/ي`, and Arabic/Persian `ک/ك`.
104. Allow multiple employee selection for Excel export, list printing, and future grouped letter issuance, but not for bulk edit or delete.
105. Permanent employee deletion requires a serious warning and retyping the personnel number.
106. Departed employees are hidden by default and are available through status filters or the archive.
107. Contracts for one employment period cannot overlap.
108. A new department assignment automatically ends the previous assignment one day before the new start date, with confirmation before saving.
109. An employee may have multiple active responsibilities.
110. Exactly one active responsibility is marked primary when active responsibilities exist.
111. Ending the primary responsibility requires choosing another active primary responsibility when another active responsibility exists.
112. One education record is marked as the primary/highest record.
113. A dependent’s national code is mandatory and globally unique through the shared Person record.
114. Validate Iranian IBAN and card numbers using appropriate algorithms; minimally validate account-number structure.
115. Allowed managed-file types are PDF, common images, Word, and Excel.
116. Maximum managed-file size is 20 MB.
117. Store files using generated unique internal names and retain original file names in the database.
118. Perform a lightweight database and required-directory health check at application startup.
119. If a previously used database is missing, do not silently create an empty replacement; show recovery and path guidance.
120. Version-one updates are performed by running a newer Inno Setup installer.
