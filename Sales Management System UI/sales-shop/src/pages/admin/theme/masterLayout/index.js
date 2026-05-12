import { memo } from 'react';
import Header from '../header';
import Footer from '../footer';

const MasterLayout = ({ children, ...props }) => {
    return (
        <>
            <Header />
            {children}
            <Footer />
        </>
    );
};

export default memo(MasterLayout);