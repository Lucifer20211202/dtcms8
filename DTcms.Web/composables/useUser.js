//获取当前用户信息
export const useUser = async(type) => {
	//当前用户信息
	const infoState = async() => {
		const state = useState('dtcms_web_user_info')
		if(!state.value) {
			const { data:res } = await useAsyncData('userInfo', () => useHttp({url: '/account/member/info'}))
			state.value =  res.value?.data
		}
		return state.value
	}
	
	//清空用户信息
	const clearState = async() => {
		const state = useState('dtcms_web_user_info')
		state.value = null
	}
	
	switch(type) {
		case 'clear': 
			return await clearState()
			break
		default:
			return await infoState()
	}
}