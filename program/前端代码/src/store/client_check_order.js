// Client在《护理模块》的《我的服务》页面，查看的某个订单

export default{
    state:{
        id: "",
        order_detail:{}

    },
    mutations:{
        //修改order的id
        setOrderID(state,newID){
            state.id=newID;
        },
        //根据ID，向后端请求获取详细信息
        RequireOrderDetail(state){
            state.aside_data=[];
        }

    }
}
