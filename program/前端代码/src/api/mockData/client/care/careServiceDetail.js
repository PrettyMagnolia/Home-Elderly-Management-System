//mock 返回的数据
//getCareServiceDetail
//client 护理模块，获取某个服务的所有detail
export default {
    getStaticData: (options) => {
        let category_id = options.body;


        if (category_id == '001') {
            var service_detail = {
                category_name: '生活照护',
                category_id: '001',
                category_logo: '',
                text_introduce: {
                    service_intro: '服务文本介绍',
                    caution: '温馨提醒',
                },
                img_introduce: '',
                service_items: [
                    {
                        id:'晨晚间护理id',
                        name: "晨晚间护理",
                        piece_price: "150",//单价
                        unit:"天",//计量单位
                        per_price:'150/天',
                        descr: "",		  //描述
                    },
                    {
                        id:'医院陪护id',
                        name: "医院陪护",
                        piece_price: "200",//单价
                        unit:"天",//计量单位
                        per_price:'200/天',
                        descr: "",		  //描述
                    },
                    {
                        id:'协助饮食id',
                        name: "协助饮食",
                        piece_price: "60",//单价
                        unit:"天",//计量单位
                        per_price:'60/天',
                        descr: "",		  //描述
                    },
                    {
                        id:'卧床全面护理id',
                        name: "卧床全面护理",
                        piece_price: "300",//单价
                        unit:"天",//计量单位
                        per_price:'300/天',
                        descr: "",		  //描述
                    },
                ],
            };
        }
        else if (category_id == '002') {

        }
        else if (category_id == '003') {

        } else if (category_id == '004') {

        } else if (category_id == '005') {

        }





        return {
            service_detail
        };
    }




}