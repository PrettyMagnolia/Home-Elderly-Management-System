//所有用户的账号相关信息

export default {
    state: {
        userName: '',//用户名
        userID: '',//用户ID
        passward: '',//密码
        token: '',
        role: ''//4种身份 
    },
    mutations: {
        //设置数据
        setData(state, data) {
            state.userName = data.userName == undefined ? state.userName : data.userName;
            state.userID = data.userID == undefined ? state.userID : data.userID;
            state.passward = data.passward == undefined ? state.passward : data.passward;
            state.token = data.token == undefined ? state.token : data.token;
            state.role = data.role == undefined ? state.role : data.role;
        },

    }
}
