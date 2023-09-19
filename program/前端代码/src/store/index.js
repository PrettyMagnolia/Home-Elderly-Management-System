import Vue from "vue";
import Vuex from 'vuex';

import aside from './aside'
import user from './user'
Vue.use(Vuex)

export default new Vuex.Store({
    modules: {
        aside, user
    }
})