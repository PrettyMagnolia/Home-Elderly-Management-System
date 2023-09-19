// clien 我的服务订单 
//返回所有的订单数据
export default {
    getStaticData: (options) => {
        //反序列化
        // let params = JSON.parse(options.body)
        // console.log("getOrderPage传参", params);
        let amount = 25
        let pageNum = 1

        let res = []
        for (let i = 0; i < amount; i++) {
            let state_rand
            if (i < 3) {
                state_rand = 0;
            }
            else if (i > 6) {
                state_rand = 2;
            }
            else {
                state_rand = 1;
            }
            //random time
            let year=Math.floor((Math.random()*2048)+1);
            let month=Math.floor((Math.random()*11)+1);
            let day=Math.floor((Math.random()*29)+1);

            let line = {
                id: 'ID' + (pageNum * amount + i).toString(),
                categoryName: '服务类型' + i.toString(),
                state: state_rand,
                work_time: (year+1).toString()+'-'+(month+1).toString()+'-'+(day+1).toString(),
                place_time: year.toString()+'-'+month.toString()+'-'+day.toString(),
                total_price: pageNum.toString() +  i.toString(),
                nurse_name: pageNum.toString() + '__' + i.toString(),
                service_items: [
                    {
                        id: '晨晚间护理id',
                        name: '晨晚间护理',
                        per_price: '150/天',
                        num: '3',
                    },
                    {
                        id: '医院陪护id',
                        name: '医院陪护',
                        per_price: '200/天',
                        num: '3',
                    },
                ],
                isNew:state_rand==1?true:false
            }
            res.push(line)
        }


        return res;


    }




}