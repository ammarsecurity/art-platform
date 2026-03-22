-- ============================================
-- منصة الفن - Art Platform Database Schema
-- MySQL 8.0+ | UTF8MB4 (full Arabic support)
-- ============================================

CREATE DATABASE IF NOT EXISTS art_platform
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE art_platform;

-- ==========================================
-- Users
-- ==========================================
CREATE TABLE users (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(100)    NOT NULL,
    email       VARCHAR(255)    NOT NULL UNIQUE,
    password_hash VARCHAR(255)  NOT NULL,
    role        ENUM('Admin','Student') NOT NULL DEFAULT 'Student',
    avatar_url  VARCHAR(500),
    bio         TEXT,
    is_active   BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at  DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  DATETIME,
    INDEX idx_email (email),
    INDEX idx_role  (role)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Categories
-- ==========================================
CREATE TABLE categories (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(100)    NOT NULL,
    slug        VARCHAR(120)    NOT NULL UNIQUE,
    description TEXT,
    image_url   VARCHAR(500),
    sort_order  INT             NOT NULL DEFAULT 0,
    is_active   BOOLEAN         NOT NULL DEFAULT TRUE,
    created_at  DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_slug (slug),
    INDEX idx_sort (sort_order)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Tags
-- ==========================================
CREATE TABLE tags (
    id    INT AUTO_INCREMENT PRIMARY KEY,
    name  VARCHAR(60)  NOT NULL,
    slug  VARCHAR(70)  NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Artworks
-- ==========================================
CREATE TABLE artworks (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    title         VARCHAR(200)    NOT NULL,
    slug          VARCHAR(220)    NOT NULL UNIQUE,
    description   TEXT,
    image_url     VARCHAR(500)    NOT NULL,
    thumbnail_url VARCHAR(500),
    medium        VARCHAR(100),
    dimensions    VARCHAR(100),
    year          SMALLINT,
    is_featured   BOOLEAN         NOT NULL DEFAULT FALSE,
    status        ENUM('Draft','Published','Archived') NOT NULL DEFAULT 'Published',
    sort_order    INT             NOT NULL DEFAULT 0,
    category_id   INT             NOT NULL,
    created_at    DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE RESTRICT,
    INDEX idx_slug       (slug),
    INDEX idx_status     (status),
    INDEX idx_featured   (is_featured),
    INDEX idx_category   (category_id),
    INDEX idx_created_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Artwork Tags (pivot)
-- ==========================================
CREATE TABLE artwork_tags (
    artwork_id INT NOT NULL,
    tag_id     INT NOT NULL,
    PRIMARY KEY (artwork_id, tag_id),
    FOREIGN KEY (artwork_id) REFERENCES artworks(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id)     REFERENCES tags(id)     ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==========================================
-- Courses
-- ==========================================
CREATE TABLE courses (
    id                INT AUTO_INCREMENT PRIMARY KEY,
    title             VARCHAR(200)    NOT NULL,
    slug              VARCHAR(220)    NOT NULL UNIQUE,
    description       LONGTEXT,
    short_description VARCHAR(500),
    thumbnail_url     VARCHAR(500),
    preview_video_url VARCHAR(500),
    price             DECIMAL(10,2)   NOT NULL DEFAULT 0.00,
    duration_minutes  INT             NOT NULL DEFAULT 0,
    level             ENUM('Beginner','Intermediate','Advanced') NOT NULL DEFAULT 'Beginner',
    status            ENUM('Draft','Published','Archived') NOT NULL DEFAULT 'Draft',
    is_featured       BOOLEAN         NOT NULL DEFAULT FALSE,
    category_id       INT             NOT NULL,
    created_at        DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at        DATETIME,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE RESTRICT,
    INDEX idx_slug     (slug),
    INDEX idx_status   (status),
    INDEX idx_featured (is_featured)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Lessons
-- ==========================================
CREATE TABLE lessons (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    title            VARCHAR(200)  NOT NULL,
    description      TEXT,
    video_url        VARCHAR(500),
    duration_minutes INT           NOT NULL DEFAULT 0,
    sort_order       INT           NOT NULL DEFAULT 0,
    is_preview       BOOLEAN       NOT NULL DEFAULT FALSE,
    is_active        BOOLEAN       NOT NULL DEFAULT TRUE,
    course_id        INT           NOT NULL,
    created_at       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE,
    INDEX idx_course    (course_id),
    INDEX idx_sort      (sort_order)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Enrollments
-- ==========================================
CREATE TABLE enrollments (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    user_id          INT           NOT NULL,
    course_id        INT           NOT NULL,
    enrolled_at      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    completed_at     DATETIME,
    progress_percent TINYINT       NOT NULL DEFAULT 0,
    UNIQUE KEY uq_user_course (user_id, course_id),
    FOREIGN KEY (user_id)   REFERENCES users(id)   ON DELETE CASCADE,
    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE,
    INDEX idx_user   (user_id),
    INDEX idx_course (course_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==========================================
-- Lesson Progress
-- ==========================================
CREATE TABLE lesson_progresses (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    user_id         INT           NOT NULL,
    lesson_id       INT           NOT NULL,
    is_completed    BOOLEAN       NOT NULL DEFAULT FALSE,
    watched_seconds INT           NOT NULL DEFAULT 0,
    completed_at    DATETIME,
    updated_at      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_user_lesson (user_id, lesson_id),
    FOREIGN KEY (user_id)   REFERENCES users(id)   ON DELETE CASCADE,
    FOREIGN KEY (lesson_id) REFERENCES lessons(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==========================================
-- Blog Posts
-- ==========================================
CREATE TABLE blog_posts (
    id                INT AUTO_INCREMENT PRIMARY KEY,
    title             VARCHAR(200)   NOT NULL,
    slug              VARCHAR(220)   NOT NULL UNIQUE,
    content           LONGTEXT       NOT NULL,
    excerpt           TEXT,
    featured_image_url VARCHAR(500),
    status            ENUM('Draft','Published','Archived') NOT NULL DEFAULT 'Draft',
    is_featured       BOOLEAN        NOT NULL DEFAULT FALSE,
    view_count        INT            NOT NULL DEFAULT 0,
    meta_title        VARCHAR(200),
    meta_description  VARCHAR(500),
    published_at      DATETIME,
    created_at        DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at        DATETIME,
    INDEX idx_slug       (slug),
    INDEX idx_status     (status),
    INDEX idx_published  (published_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Contact Messages
-- ==========================================
CREATE TABLE contact_messages (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    name       VARCHAR(100)   NOT NULL,
    email      VARCHAR(255)   NOT NULL,
    phone      VARCHAR(30),
    subject    VARCHAR(200)   NOT NULL,
    message    TEXT           NOT NULL,
    is_read    BOOLEAN        NOT NULL DEFAULT FALSE,
    created_at DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_is_read   (is_read),
    INDEX idx_created   (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Site Settings
-- ==========================================
CREATE TABLE site_settings (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    `key`       VARCHAR(100)   NOT NULL UNIQUE,
    value       TEXT           NOT NULL,
    description VARCHAR(300),
    INDEX idx_key (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- Seed Data
-- ==========================================
INSERT INTO site_settings (`key`, value, description) VALUES
('site_name',         'منصة الفن',                       'اسم الموقع'),
('site_description',  'منصة للفنون والتعلم',              'وصف الموقع'),
('contact_email',     'info@artplatform.com',             'بريد التواصل'),
('hero_title',        'استكشف عالم الفن',                 'عنوان البانر الرئيسي'),
('hero_subtitle',     'تعلم، استلهم، وأبدع مع أفضل الفنانين', 'النص الفرعي'),
('social_instagram',  '',                                 'رابط إنستغرام'),
('social_twitter',    '',                                 'رابط تويتر'),
('social_youtube',    '',                                 'رابط يوتيوب');

INSERT INTO categories (name, slug, sort_order) VALUES
('لوحات زيتية',       'oil-paintings',  1),
('رسم رقمي',          'digital-art',    2),
('تصوير فوتوغرافي',   'photography',    3),
('فن تجريدي',         'abstract-art',   4),
('خط عربي',           'calligraphy',    5);

-- Admin user (password: Admin@1234)
INSERT INTO users (name, email, password_hash, role) VALUES
('مدير النظام', 'admin@artplatform.com', '$2a$11$dummy_hash_replace_with_real', 'Admin');
