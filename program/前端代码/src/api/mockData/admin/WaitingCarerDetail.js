
export default{
    getStaticData:()=>{
        let WaitingCarerDetail_table={
            list:[
              [
              {
                id: "0",
                name: "张三",
                sex: "女",
                age: "53",
                date: "2022.2.26",
                state:"已完成审核",
              },
              {
                id: "1",
                name: "李四",
                sex: "男",
                age: "62",
                date: "2022.5.6",
                state:"待办审核",
              },
              {
                id: "2",
                name: "王五",
                sex: "女",
                age: "51",
                date: "2022.7.2",
                state:"已完成审核",
              },
            ],
            [
              {
                id: "0",
                name: "张三",
                sex: "女",
                age: "53",
                date: "2022.2.26",
                state:"已完成审核",
              },
              {
                id: "2",
                name: "王五",
                sex: "女",
                age: "51",
                date: "2022.7.2",
                state:"已完成审核",
              },
            ],
            [
              {
                id: "1",
                name: "李四",
                sex: "男",
                age: "62",
                date: "2022.5.6",
                state:"待办审核",
              },
            ],
        ]
    }
        return{
            WaitingCarerDetail_table
        }
    }
}