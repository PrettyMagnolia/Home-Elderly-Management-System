//mock 返回的数据
//返回对应order的订单详情

export default {
    getStaticData: (options) => {

        //options.body是传来的参数
        console.log("getOrderDetail的传参", options.body);
        let res = {
            client_name: "kooriookami",
            client_phone: "18100000000",
            order_address: "同济大学-嘉定校区 嘉定区曹安公路4800号, 上海市",
            contact_address: "下北泽 东京23区日本 世田谷区",
            category_name: "生活照护",

            work_time: "XXXX-XX-XX",

            total_price: "$1145",
            order_state: "已完成",  //正在进行，已发布，已完成
            service_items: [
                {
                    id: '晨晚间护理id',
                    name: '晨晚间护理',
                    piece_price: '150',
                    unit: '天',
                    per_price: '150/天',
                    num: '3',
                },
                {
                    id: '医院陪护id',
                    name: '医院陪护',
                    piece_price: '200',
                    unit: '天',
                    per_price: '200/天',
                    num: '3',
                },
            ],
            //自理能力
            self_care_situation: "完全自理",


            //护理需求方向
            careDirect: "长期居家",

            //接单的护工列表，等待确认
            accessible_nurses: [
                {
                    id: "A11",
                    name: "A1",
                    gender: "男",
                    avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
                    age: 11,
                    isChecked: false,
                },
                {
                    id: "B11",
                    name: "B1",
                    gender: "男",
                    avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
                    age: 22,
                    isChecked: false,
                },
                {
                    id: "C11",
                    name: "C1",
                    gender: "男",
                    avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
                    age: 33,
                    isChecked: false,
                },
                {
                    id: "D11",
                    name: "D1",
                    gender: "男",
                    avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
                    age: 44,
                    isChecked: false,
                },
            ],

            //正式接单的护工的简介
            nurse_intro: {
                avatar: "https://s2.loli.net/2022/07/15/iDq7y6rCPkoGAcv.png",
                name: "护工XXX",
                age: "43",
                gender: "男",
                id: "114514",
            },




        };

        return res;
    }




}