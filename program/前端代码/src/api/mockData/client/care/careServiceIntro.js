//mock 返回的数据
//getIndexService()
//client 护理模块，首页介绍，只有简介
export default{
    getStaticData:()=>{
        let List=[
            {
                service_uid:'001',
                service_name:"生活照护",
                service_logo:"https://s2.loli.net/2022/07/30/2pHje69u5qXiFZP.png",
            },
            {
                service_uid:'002',
                service_name:"失能介护",
                service_logo:"https://s2.loli.net/2022/07/30/QM5ilNFJpSnD1e7.png",
            },
            {
                service_uid:'003',
                service_name:"老年保健",
                service_logo:"https://s2.loli.net/2022/07/30/ZacfGgvMeNiykKp.png",
            },
            {
                service_uid:'004',
                service_name:"家庭保洁",
                service_logo:"https://s2.loli.net/2022/07/30/mSzhgRoC8u3teyM.png",
            },
            {
                service_uid:'005',
                service_name:"家电清洗",
                service_logo:"https://s2.loli.net/2022/07/26/6ePixSTZRLHbUcz.png",
                // detail_intro:"家电清洗"+"的详细介绍",
            },

        ];
        


        return {
                List
            };
    }




}