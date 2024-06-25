<template>
	<div class="login-box">
		<div class="head-box">
			<h3>{{props.title}}</h3>
			<NuxtLink to="/account/login">
				已有账号，立即登录
				<i class="iconfont icon-arrow-right"></i>
			</NuxtLink>
		</div>
		<el-tabs v-model="datas.activeName" @tab-change="handleTabChange">
			<el-tab-pane label="用户名注册" name="name">
				<el-form ref="userFormRef" :model="datas.userForm" :rules="datas.rules" @submit.native.prevent>
					<el-form-item prop="userName" class="input-box">
						<el-input type="text" v-model="datas.userForm.userName" placeholder="请输入账户名" :prefix-icon="ElIconUser"></el-input>
					</el-form-item>
					<el-form-item prop="password" class="input-box">
						<el-input type="password" v-model="datas.userForm.password" placeholder="请输入密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item prop="codeValue" class="input-box">
						<el-input type="text" v-model="datas.userForm.codeValue" placeholder="请输入验证码" :prefix-icon="ElIconCellphone">
							<template #append>
								<el-image @click="initData" class="code" :src="datas.imgSrc" alt="点击切换验证码" />
							</template>
						</el-input>
					</el-form-item>
					<div class="foot-box" v-if="memberConfig.regRules===1">
						<el-checkbox v-model="datas.agree" label="同意隐私协议" />
						<a @click="datas.agreeDialog=true">《用户隐私全文》</a>
					</div>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" :loading="datas.btnLoading" @click="submitForm">确认注册</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
			
			<el-tab-pane label="手机注册" name="mobile">
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
					<div class="foot-box" v-if="memberConfig.regRules===1">
						<el-checkbox v-model="datas.agree" label="同意隐私协议" />
						<a @click="datas.agreeDialog=true">《用户隐私全文》</a>
					</div>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" :loading="datas.btnLoading" @click="submitForm">确认注册</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
			
			<el-tab-pane label="邮箱注册" name="email">
				<el-form ref="emailFormRef" :model="datas.emailForm" :rules="datas.rules" @submit.native.prevent>
					<el-form-item prop="email" class="input-box">
						<el-input type="text" v-model="datas.emailForm.email" placeholder="请输入邮箱账户" :prefix-icon="ElIconUser"></el-input>
					</el-form-item>
					<el-form-item prop="codeValue" class="input-box">
						<el-input type="text" v-model="datas.emailForm.codeValue" placeholder="请输入邮箱验证码" :prefix-icon="ElIconCellphone">
							<template #append>
								<el-button v-if="datas.timer>0">重新获取({{datas.timer}}s)</el-button>
								<el-button v-else @click="sendEmailCode">获取验证码</el-button>
							</template>
						</el-input>
					</el-form-item>
					<div class="foot-box" v-if="memberConfig.regRules===1">
						<el-checkbox v-model="datas.agree" label="同意隐私协议" />
						<a @click="datas.agreeDialog=true">《用户隐私全文》</a>
					</div>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" :loading="datas.btnLoading" @click="submitForm">确认注册</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
		</el-tabs>
	</div>
	<!--协议全文-->
	<el-dialog v-model="datas.agreeDialog" title="用户隐私协议" width="80%" draggable :fullscreen="false">
		<div style="max-height:70vh;overflow-y:auto;" v-html="memberConfig.regRulesTxt"></div>
		<template #footer>
			<div class="dialog-footer">
				<el-button @click="handleAgree(0)">不同意</el-button>
				<el-button type="primary" @click="handleAgree(1)">同意</el-button>
		      </div>
		</template>
	</el-dialog>
</template>

<script setup>
	const route = useRoute()
	const tokenObj = useToken()
	const userFormRef = ref(null)
	const phoneFormRef = ref(null)
	const emailFormRef = ref(null)
	//获取当前站点信息
	const siteConfig = await useSite('site')
	const memberConfig = await useSite('member')
	//接收props传值
	const props = defineProps({
		//标题
		title: {
			type: String,
			default: '注册'
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
		agreeDialog: false, //显示协议全文
		agree: false, //同意协议
		userForm: {
			userName: null,
			password: null,
			codeKey: null,
			codeValue: null,
		},
		phoneForm: {
			phone: null,
			codeKey: null,
			codeValue: null,
		},
		//邮箱注册
		emailForm: {
			email: null,
			password: null,
			codeKey: null,
			codeValue: null,
		},
		rules: {
			userName: [
				{ required: true, message: '请输入登录用户名', trigger: 'blur' },
				{ min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' },
			],
			email: [
				{ required: true, message: '请输入邮箱账户', trigger: 'blur' },
				{ pattern: /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/, message: '邮箱格式有误', trigger: 'blur' },
			],
			phone: [
				{ required: true, message: '请输入手机号码', trigger: 'blur' },
				{ pattern: /^1[0-9]{10}$/, message: '手机号码格式不正确', trigger: 'blur' },
			],
			password: [
				{ required: true, message: '请输入登录密码', trigger: 'blur' },
				{ min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' },
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
				datas.userForm.codeKey = res.data.key
				datas.imgSrc = res.data.data
			}
		})
	}
	//切换Tab事件
	const handleTabChange = () => {
		//清除其它表单验证
		if(datas.activeName == 'name') {
			phoneFormRef.value.clearValidate()
			emailFormRef.value.clearValidate()
		} else if(datas.activeName == 'mobile') {
			userFormRef.value.clearValidate()
			emailFormRef.value.clearValidate()
		} else {
			userFormRef.value.clearValidate()
			phoneFormRef.value.clearValidate()
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
			url: `/verifycode/mobile/${datas.phoneForm.phone}?check=0`,
			success(res) {
				datas.timer = 120; //设置120秒
				countDown() //开始倒计时
				datas.phoneForm.codeKey = res.data.codeKey
			}
		})
	}
	//发送邮件验证码
	const sendEmailCode = () => {
		if (!/(^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,})$)/.test(datas.emailForm.email)) {
			ElMessage({
				message: '提示，请输入正确的邮箱账户',
				type: 'warning',
			})
			return
		}
		//发送邮件
		useHttp({
			url: `/verifycode/email/${datas.emailForm.email}?check=0`,
			success(res) {
				datas.timer = 120 //设置120秒
				countDown() //开始倒计时
				datas.emailForm.codeKey = res.data.codeKey
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
	//同意或不同意协议
	const handleAgree = (val) => {
		datas.agree = val > 0
		datas.agreeDialog = false
	}
	//提交表单
	const submitForm = () => {
		if (memberConfig.regRules===1 && !datas.agree) {
			ElMessage.warning('提示，请先阅读并同意隐私协议')
			return
		}
		let formObj = null
		let model = null
		//判断普通或验证码登录
		if(datas.activeName === 'name') {
			formObj = userFormRef.value
			model = datas.userForm
		} else if(datas.activeName === 'mobile') {
			formObj = phoneFormRef.value
			model = datas.phoneForm
		} else {
			formObj = emailFormRef.value
			model = datas.emailForm
		}
		//表单验证
		formObj.validate((valid) => {
			if (valid) {
				//取得推荐人的ID
				const refId = useState('dtcms_web_ref').value
				let sendUrl = `/account/member/register`
				if(refId) {
					sendUrl += `?rid=${refId}`
				}
				//获取当前站点ID
				model.siteId = siteConfig.id
				useHttp({
					method: 'post',
					url: sendUrl,
					data: model,
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						ElMessageBox.confirm('注册成功，是否登录会员账户？', '提示', {
							confirmButtonText: '确定',
							cancelButtonText: '取消',
							type: 'success',
						}).then(() => {
							navigateTo('/account/login')
						}).catch(() => {
							navigateTo('/')
						})
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
	
	//初始化数据
	initData()
</script>