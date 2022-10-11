import { NextPage } from "next";
import Link from "next/link";
import { Anchor } from "./anchor";
// import styles from '../styles/Main.module.css'

export const Footer: NextPage = () => {
    return(
        <>
            <footer className="flex items-center justify-center h-10 border-t-2 border-medium bg-lightest">
                <Anchor href="https://rgamer43.github.io">Made by Rgamer43</Anchor>
            </footer>
        </>
    )
}