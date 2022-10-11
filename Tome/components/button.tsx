import { NextPage } from "next"

type Props = {
    onClick?: any,
    children: any,
    className?: string,
    type?: "button" | "submit" | "reset"
}

export const Button: NextPage<Props> = (props) => {
    return(
        <> 
            <button onClick={props.onClick} type={props.type} className={"bg-lighter hover:bg-lighterAlt active:bg-medium border-darkest border-1 rounded min-w-max pl-1 pr-1 hover:shadow-sm " + props.className}>
                {props.children}
            </button>
        </>
    )
}