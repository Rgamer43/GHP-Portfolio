import { NextPage } from "next";
import Link from "next/link";
// import styles from '../styles/Main.module.css'

type Props = {
    href: any,
    children: any,
    query?: any,
    className?: string
}

export const Anchor: NextPage<Props> = (props) => {
    const className = props.className + " hover:underline"

    if(props.href.startsWith("http://") || props.href.startsWith("https://"))
        return(
            <>
                <a href={props.href} className={className}>{props.children}</a>
            </>
        )
    else 
        return(
            <> 
                <Link href={{ pathname: props.href, query: props.query}} passHref>
                    <a className={className}>{props.children}</a>
                </Link>
            </>
        )
}