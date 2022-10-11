import '../styles/globals.css'
import type { AppProps } from 'next/app'
import { NavHeader } from '../components/navheader'
import { Footer } from '../components/footer'
import { init, testPing } from '../lib/networkmanager'
import { SessionProvider } from "next-auth/react"
import { Button } from '../components/button';

function MyApp({ Component, pageProps: {session, ...pageProps}, }: AppProps) {
  init()

  return (
    <SessionProvider session={session}>
      <span className="flex flex-col min-h-screen">
        <NavHeader></NavHeader>
        {
          //<Button onClick={testPing} className={''}>Test Ping</Button>
        }
        <span className="flex-grow">
          <Component {...pageProps} />
        </span>
        <Footer></Footer>
      </span>
    </SessionProvider>
  )
}

export default MyApp
