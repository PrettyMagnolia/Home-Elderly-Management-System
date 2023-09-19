//mock 返回的数据
//返回某个护工的简介

export default {
    getStaticData: (options) => {

        //options.body是传来的参数
        console.log("getOrderDetail的传参", options.body);
        let res = {
            avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
            name: "护工XXX",
            age: "43",
            gender: "男",
            id: "114514",
        };

        return res;
    }




}