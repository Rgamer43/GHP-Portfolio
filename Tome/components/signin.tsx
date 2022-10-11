import { Button } from "./button";
import { verifySignIn, handleSignOut, shouldCreateAccount, handleSignIn } from '../lib/networkmanager';
import { useSession, signIn, signOut, getSession } from 'next-auth/react';
import { useEffect } from "react";

type Props = {
    [x: string]: string,
}

export var useSessionData: any

export function SignIn(props={className: "", textClassName: "", signOut: false}) {
    const {data: session, status } = useSession()
    useSessionData = {data: session, status}
    
    const handleSignOutWrapper = (e: any) => {
        e.preventDefault()
        handleSignOut()
    }

    const goToProfile = () => {
        window.location.href = 'profile'
    }

    useEffect(() => {
        if(getSession() !== null && getSession() !== undefined) verifySignIn()
    })

    if(session) {
        return(
            <>
                <Button onClick={props.signOut ? handleSignOutWrapper : goToProfile} className={props.className}>
                    <h1 className={'text-2xl ' + props.textClassName}>{props.signOut ? "Sign Out": "Go to profile"}</h1>
                </Button>
            </>
        )
    } else {
        return(
            <>
    
                <Button onClick={handleSignIn} className={props.className}>
                    <h1 className={'text-2xl ' + props.textClassName}>Sign In</h1>
                </Button>
            </>
        )
    }
}