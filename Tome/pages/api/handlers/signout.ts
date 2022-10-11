import { removeVerifiedEmail } from "../database/verifiedEmails"

export default function signOut(email: string) {
    removeVerifiedEmail(email)
    console.log("Removed verified email")
}