import { NextApiRequest, NextApiResponse } from 'next'
import NextAuth, { Awaitable, NextAuthOptions } from 'next-auth'
import { JWT } from 'next-auth/jwt'
import GoogleProvider from 'next-auth/providers/google'
import { addVerifiedEmail, includesVerifiedEmail } from '../database/verifiedEmails';

export const options: NextAuthOptions = {
    providers: [
        GoogleProvider({
            clientId: String(process.env.GOOGLE_ID),
            clientSecret: String(process.env.GOOGLE_SECRET)
        })
    ],
    callbacks: {
        async jwt({token, user, account, profile, isNewUser}): Promise<JWT> {
            console.log("User signed in (nextauth)")

            if(account) {
                token.accessToken = account.accessToken
            }
            
            if(token.email !== undefined && token.email !== null) {
                if(!(await includesVerifiedEmail(token.email))) {
                    addVerifiedEmail(token.email)
                } else console.log("VerifiedEmails already includes " + token.email)
            }
            
            return token
        }
    }
}

export default (req: NextApiRequest, res: NextApiResponse<any>) => NextAuth(req, res, options)