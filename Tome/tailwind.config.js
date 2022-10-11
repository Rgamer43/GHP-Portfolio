/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./pages/**/*.{js,ts,jsx,tsx}",
    "./components/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        lightest: '#F8F5F2',
        lighter: '#E8DED3',
        lighterAlt: '#C9B2A0',
        lighterSubtle: '#eeebe8',
        light: '#C9B2A0',
        medium: '#9A7861',
        dark: '#4B4842',
        darkest: '#343836'
      },
    },
  },
  plugins: [],
}
