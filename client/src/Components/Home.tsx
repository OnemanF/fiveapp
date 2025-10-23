import { Outlet } from "react-router-dom";
import Layout from "./Layout";

export default function Home() {
    return (
        <Layout>
            <Outlet />
        </Layout>
    );
}
