BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Vacation" (
	"id"	INTEGER,
	"User"	INTEGER NOT NULL,
	"name"	TEXT NOT NULL,
	"description"	TEXT,
	"start"	INTEGER NOT NULL,
	"end"	INTEGER NOT NULL,
	PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Vacation_Photo" (
	"id"	INTEGER,
	"Vacation"	INTEGER NOT NULL,
	"description"	TEXT,
	"path"	TEXT NOT NULL,
	"date"	INTEGER NOT NULL,
	PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "User" (
	"id"	INTEGER,
	"username"	TEXT UNIQUE,
	"email"	TEXT UNIQUE,
	"password"	TEXT NOT NULL,
	"description"	TEXT,
	"profile_picture"	TEXT,
	"created_at"	INTEGER NOT NULL,
	PRIMARY KEY("id" AUTOINCREMENT)
);
COMMIT;
