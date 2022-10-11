/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: false,
  swcMinify: true,
  env: {
    //Google OAuth
    GOOGLE_ID: "Redacted",
    GOOGLE_SECRET: "Redacted",

    //NextAuth
    NEXTAUTH_URL: "http://localhost:3000",
    NEXTAUTH_SECRET: "Redacted",

    //NextJS
    DEV: true,
    PORT: 3000,

    //MongoDB
    MONGO_URI: "Redacted",
    MONGO_DB: "storage",

    //Posts & Comments
    WORD_MIN: 100,
    WORD_MAX: 20000,
    COMMENT_CHAR_MIN: 4,
    COMMENT_CHAR_MAX: 1000,
    CARD_BODY_MAX: 100,
    CARD_TITLE_MAX: 50,

    //Algorithms
    SEARCH_LIMIT: 25,

    //Popularity
    POP_VIEW_WEIGHT: 1,
    POP_COMMENT_WEIGHT: 1.2,
    POP_LIKE_WEIGHT: 1.35,
    POP_DISLIKE_WEIGHT: 0.9,

    //Top
    TOP_VIEW_WEIGHT: 1.1,
    TOP_COMMENT_WEIGHT: 1.15,
    TOP_LIKE_WEIGHT: 1.45,
    TOP_DISLIKE_WEIGHT: 0.85,

    //Hot
    HOT_VIEW_WEIGHT: 1.3,
    HOT_COMMENT_WEIGHT: 1.45,
    HOT_LIKE_WEIGHT: 1.15,
    HOT_DISLIKE_WEIGHT: 0.9,

    //Images
    DEFAULT_THUMBNAIL: "https://images.ctfassets.net/3s5io6mnxfqz/PHpqtygGNAxEOA3374N7U/900ba5fff69cfce44ad0357957bdf476/AdobeStock_279838076.jpeg",
    DEFAULT_HEADER_IMAGE: "https://i.ytimg.com/vi/w8MrqRHu2Es/maxresdefault.jpg"
  }
}

module.exports = nextConfig
