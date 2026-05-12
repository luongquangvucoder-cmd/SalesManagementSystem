import { ROUTERS } from "./untils/router";
import DashboardPage from "./pages/admin/dashboardPage";
import { Routes, Route } from "react-router-dom";
import MasterLayout from "./pages/admin/theme/masterLayout";
import CategoryPage from "./pages/admin/categoryPage";

const renderAdminRoutes = () => {
    const adminRouters = [
        {
            path: ROUTERS.ADMIN.DASHBOARD,
            component: <DashboardPage />,
        },
        {
            path: ROUTERS.ADMIN.CATEGORY,
            component: <CategoryPage />,
        }
    ]

    return (
        <MasterLayout>
            <Routes>
                {
                    adminRouters.map((item, key) => (
                        <Route key={key} path={item.path} element={item.component} />
                    ))
                }
            </Routes>
        </MasterLayout>
    )
}

const RouterCustom = () => {
    return renderAdminRoutes();
};

export default RouterCustom;