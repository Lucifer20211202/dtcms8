//获取或写入Token
export const useToken = () => {
	const tokenKey = 'dtcms_web_token'
	const tokenObj = useCookie(tokenKey, { maxAge: 60*60*24*30 })
	
	//获取Token
	const token = tokenObj.value
	//写入Token
	const set = (accessToken, refreshToken) => {
		tokenObj.value = { accessToken, refreshToken }
	}
	//删除Token
	const remove = () => {
		tokenObj.value = null
	}
	
	return {
		token,
		set,
		remove
	}
}