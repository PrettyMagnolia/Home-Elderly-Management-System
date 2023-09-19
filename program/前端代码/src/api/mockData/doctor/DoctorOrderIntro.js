//mock 返回的数据
let List=[]
export default{
    getDoctorServiceData:()=>{
        for(let i=0;i<16;i++){
            List.push(
                {
                    order_uid:i,
                    order_person:"老人"+i.toString(),
                    order_type:"医疗服务"+i.toString(),
                    order_logo:"https://www.minitu.cn/wp-content/uploads/2021/03/wjt1.png",
                    detail_intro:"申请的详细要求",
                }
            )
        }

        return {
                List
        };
    }




}