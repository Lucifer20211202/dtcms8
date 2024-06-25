<template>
	<div class="login-box">
		<div class="head-box">
			<h3>{{props.title}}</h3>
			<NuxtLink to="/account/register">
				没有账号？立即注册
				<i class="iconfont icon-arrow-right"></i>
			</NuxtLink>
		</div>
		<el-tabs v-model="datas.activeName" @tab-change="handleTabChange">
			<el-tab-pane label="密码登录" name="name">
				<el-form ref="loginFormRef" :model="datas.loginForm" :rules="datas.rules" @submit.native.prevent>
					<el-form-item prop="userName" class="input-box">
						<el-input type="text" v-model="datas.loginForm.userName" placeholder="请输入账户名" :prefix-icon="ElIconUser"></el-input>
					</el-form-item>
					<el-form-item prop="password" class="input-box">
						<el-input type="password" v-model="datas.loginForm.password" placeholder="请输入密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item prop="codeValue" class="input-box">
						<el-input type="text" v-model="datas.loginForm.codeValue" placeholder="请输入验证码" :prefix-icon="ElIconCellphone">
							<template #append>
								<el-image @click="initData" class="code" :src="datas.imgSrc" alt="点击切换验证码" />
							</template>
						</el-input>
					</el-form-item>
					<div class="foot-box">
						<NuxtLink to="/account/repassword">忘记密码？立即找回</NuxtLink>
					</div>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" :loading="datas.btnLoading" @click="submitForm">点击登录</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
			<el-tab-pane label="验证码登录" name="mobile">
				<el-form ref="phoneFormRef" :model="datas.phoneForm" :rules="datas.rules" @submit.native.prevent>
					<el-form-item prop="phone" class="input-box">
						<el-input type="text" v-model="datas.phoneForm.phone" placeholder="请输入手机号码" :prefix-icon="ElIconUser"></el-input>
					</el-form-item>
					<el-form-item prop="codeValue" class="input-box">
						<el-input type="text" v-model="datas.phoneForm.codeValue" placeholder="请输入手机验证码" :prefix-icon="ElIconCellphone">
							<template #append>
								<el-button v-if="datas.timer>0">重新获取({{datas.timer}}s)</el-button>
								<el-button v-else @click="sendPhoneCode">获取验证码</el-button>
							</template>
						</el-input>
					</el-form-item>
					<div class="foot-box">
						<NuxtLink to="/account/repassword">忘记密码？立即找回</NuxtLink>
					</div>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" :loading="datas.btnLoading" @click="submitForm">点击登录</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
		</el-tabs>
		<template v-if="datas.oauthList.length>0">
			<div class="line-box">社交账号登录</div>
			<div class="oauth-box">
				<a v-for="(item,index) in datas.oauthList" :key="index" @click="redirectUrl(item.provider)">
					<img :src="common.loadFile(item.imgUrl)" />
				</a>
			</div>
		</template>
	</div>
</template>

<script setup>
	const route = useRoute()
	const tokenObj = useToken()
	const loginFormRef = ref(null)
	const phoneFormRef = ref(null)
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//通知父组件响应
	const emits = defineEmits(['change'])
	//接收props传值
	const props = defineProps({
		//标题
		title: {
			type: String,
			default: '登录'
		},
		//成功后是否跳转
		redirect: {
			type: Boolean,
			default: () => {
				return true
			}
		}
	})
	//声明变量
	const datas = reactive({
		activeName: 'name',
		btnLoading: false,
		imgSrc: '',
		timer: 0, //计时器
		oauthList: [],
		loginForm: {
			userName: 'test',
			password: 'test888',
			codeKey: null,
			codeValue: null,
		},
		phoneForm: {
			phone: null,
			codeKey: null,
			codeValue: null,
		},
		rules: {
			userName: [
				{ required: true, message: '请输入登录用户名', trigger: 'blur' },
				{ min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' },
			],
			password: [
				{ required: true, message: '请输入登录密码', trigger: 'blur' },
				{ min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' },
			],
			phone: [
				{ required: true, message: '请输入手机号码', trigger: 'blur' },
				{ pattern: /^1[0-9]{10}$/, message: '手机号码格式不正确', trigger: 'blur' },
			],
			codeValue: [
				{ required: true, message: '请输入验证码', trigger: 'blur' },
				{ min: 4, max: 8, message: '请输入 4 到 8 位的验证码', trigger: 'blur' },
			],
		},
	})
	
	//初始化数据
	const initData = () => {
		//验证码
		useHttp({
			url: '/verifycode',
			success(res) {
				datas.loginForm.codeKey = res.data.key
				datas.imgSrc = res.data.data
			}
		})
		//授权登录
		useHttp({
			url: `/client/site/oauth/view/0?siteId=${siteConfig.id}&types=web`,
			success(res) {
				datas.oauthList = res.data
			}
		})
	}
	//提交表单
	const submitForm = () => {
		let formObj = null
		let model = null
		let sendUrl = null
		//判断普通或验证码登录
		if(datas.activeName === 'name') {
			formObj = loginFormRef.value
			model = datas.loginForm
			sendUrl = '/auth/login'
		} else {
			formObj = phoneFormRef.value
			model = datas.phoneForm
			sendUrl = '/auth/login/phone'
		}
		//表单验证
		formObj.validate((valid) => {
			if (valid) {
				useHttp({
					method: 'post',
					url: sendUrl,
					data: model,
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						//保存Token，以及跳转
						tokenObj.set(res.data.accessToken, res.data.refreshToken)
						if(props.redirect) {
							if(route.query.url) {
								navigateTo(route.query.url)
							} else {
								navigateTo('/account')
							}
						} else {
							//通知回调
							emits('change')
						}
						
					},
					complete() {
						datas.btnLoading = false
					}
				})
			} else {
				return false
			}
		})
	}
	//切换Tab事件
	const handleTabChange = () => {
		//清除其它表单验证
		if(datas.activeName == 'name') {
			phoneFormRef.value.clearValidate()
		} else {
			loginFormRef.value.clearValidate()
		}
	}
	//获取手机验证码
	const sendPhoneCode = () => {
		if (!/(^1[3|4|5|7|8][0-9]{9}$)/.test(datas.phoneForm.phone)) {
			ElMessage({
				message: '提示，请输入正确的手机号码',
				type: 'warning'
			})
			return
		}
		//发送短信
		useHttp({
			url: `/verifycode/mobile/${datas.phoneForm.phone}`,
			success(res) {
				datas.timer = 120; //设置120秒
				countDown() //开始倒计时
				datas.phoneForm.codeKey = res.data.codeKey
			}
		})
	}
	//获取验证码计时器
	const countDown = () => {
		if (datas.timer > 0) {
			datas.timer--
			setTimeout(() => {
				countDown()
			}, 1000)
		}
	}
	//跳转授权登录
	const redirectUrl = (provider) => {
		let href = `${window.location.protocol}//${window.location.host}/account/login?provider=${provider}`
		let uri = encodeURIComponent(href)
		//发送请求
		useHttp({
			url: `/oauth/redirect?siteId=${siteConfig.id}&provider=${provider}&redirectUri=${uri}`,
			success(res) {
				navigateTo(res.data)
			}
		})
	}
	//授权登录
	const submitOAuth = (provider, code) => {
		let href = `${window.location.protocol}//${window.location.host}/account/login?provider=${provider}`
		let uri = encodeURIComponent(href)
		//发送请求
		useHttp({
			url: `/oauth/login`,
			method: 'post',
			loading: true,
			data: {
				siteId: siteConfig.id,
				provider: provider,
				code: code,
				redirectUri: uri,
			},
			success(res) {
				//保存Token，以及跳转
				tokenObj.set(res.data.accessToken, res.data.refreshToken)
				if(props.redirect) {
					if(route.query.url) {
						navigateTo(route.query.url)
					} else {
						navigateTo('/account')
					}
				} else {
					//通知回调
					emits('change')
				}
			}
		})
	}
	
	//初始化数据
	initData()
</script>