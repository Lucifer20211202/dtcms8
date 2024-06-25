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
			<el-tab-pane label="手机取回" name="phone">
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
					<el-form-item prop="newPassword">
						<el-input type="password" v-model="datas.phoneForm.newPassword" placeholder="请输入新密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item prop="confirmPassword">
						<el-input type="password" v-model="datas.phoneForm.confirmPassword" placeholder="请再次输入新密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" @click="submitForm">确认修改</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
			
			<el-tab-pane label="邮箱取回" name="email">
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
					<el-form-item prop="newPassword">
						<el-input type="password" v-model="datas.emailForm.newPassword" placeholder="请输入新密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item prop="confirmPassword">
						<el-input type="password" v-model="datas.emailForm.confirmPassword" placeholder="请再次输入新密码" :prefix-icon="ElIconLock"></el-input>
					</el-form-item>
					<el-form-item class="button-box">
						<el-button type="primary" native-type="submit" @click="submitForm">确认修改</el-button>
					</el-form-item>
				</el-form>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script setup>
	const phoneFormRef = ref(null)
	const emailFormRef = ref(null)
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//接收props传值
	const props = defineProps({
		//标题
		title: {
			type: String,
			default: '重置密码'
		},
	})
	//自定义表单验证，必须放在datas之前
	const validatePass = (rule, value, callback) => {
		let newPassword = datas.phoneForm.newPassword
		if(datas.activeName == 'email') {
			newPassword = datas.emailForm.newPassword
		}
		if (value == null || value == '') {
			callback(new Error('请输入确认密码'))
		} else if (value !== newPassword) {
			callback(new Error("两次输入密码不一致"))
		} else {
			callback()
		}
	}
	//声明变量
	const datas = reactive({
		activeName: 'phone',
		btnLoading: false,
		timer: 0, //计时器
		phoneForm: {
			phone: null,
			newPassword: null,
			confirmPassword: null,
			codeKey: null,
			codeValue: null
		},
		emailForm: {
			email: null,
			newPassword: null,
			confirmPassword: null,
			codeKey: null,
			codeValue: null
		},
		rules: {
			email: [
				{ required: true, message: '请输入邮箱账户', trigger: 'blur' },
				{ pattern: /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/, message: '邮箱格式有误', trigger: 'blur' }
			],
			phone: [
				{ required: true, message: '请输入手机号码', trigger: 'blur' },
				{ pattern: /^1[0-9]{10}$/, message: '手机号码格式不正确', trigger: 'blur' }
			],
			codeValue: [
				{ required: true, message: '请输入验证码', trigger: 'blur' },
				{ lmin: 4, max: 8, message: '请输入 4 到 8 位的验证码', trigger: 'blur' }
			],
			newPassword: [
				{ required: true, message: '请输入新密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$/, message: '必须英文字母数字组合', trigger: 'blur' }
			],
			confirmPassword: [
				{ required: true, message: '请输入确认密码', trigger: 'blur' },
				{ required: true, validator: validatePass, trigger: 'blur' }
			],
		},
	})
	
	//提交表单
	const submitForm = () => {
		let formObj = null
		let model = null
		//判断普通或验证码登录
		if(datas.activeName === 'phone') {
			formObj = phoneFormRef.value
			model = datas.phoneForm
		} else {
			formObj = emailFormRef.value
			model = datas.emailForm
		}
		//表单验证
		formObj.validate((valid) => {
			if (valid) {
				useHttp({
					method: 'post',
					url: '/auth/reset',
					data: model,
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						ElMessageBox.confirm('找回成功，是否登录会员账户？', '提示', {
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
	//切换Tab事件
	const handleTabChange = () => {
		//清除其它表单验证
		if(datas.activeName == 'phone') {
			emailFormRef.value.clearValidate()
		} else {
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
			url: `/verifycode/mobile/${datas.phoneForm.phone}`,
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
			url: `/verifycode/email/${datas.emailForm.email}`,
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
	
</script>