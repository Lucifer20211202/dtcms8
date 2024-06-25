<template>
	<common-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
		<el-tab-pane label="账户充值" name="info">
			<el-form-item label="用户名">
				<span>{{userInfo.userName}}</span>
			</el-form-item>
			<el-form-item label="账户余额">
				{{userInfo.amount}}元
			</el-form-item>
			<el-form-item prop="amount" label="充值金额">
				<el-input v-model="datas.model.amount" placeholder="请输入充值金额(元)">
					<template #append>元</template>
				</el-input>
			</el-form-item>
			<el-form-item prop="paymentId" label="支付方式">
				<el-select v-model="datas.model.paymentId" placeholder="请选择支付方式">
					<el-option v-for="item in datas.paymentList"
						:key="item.id"
						:label="item.title"
						:value="item.id">
					</el-option>
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" :loading="datas.btnLoading" @click="submitForm">确认提交</el-button>
				<el-button @click="common.back(-1)">返回上一页</el-button>
			</el-form-item>
		</el-tab-pane>
	</common-form-box>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `账户充值 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//获取当前用户信息
	const userInfo = await useUser('info')
	//Ref对象
	const editFormRef = ref(null)
	//获取路由信息
	const route = useRoute()
	//通知父组件响应
	const emits = defineEmits(['change'])
	//定义页面属性
	const datas = reactive({
		loading: false,
		btnLoading: false,
		paymentList: [],
		model: {
			paymentId: null,
			amount: null
		},
		rules: {
			amount: [
				{ required: true, message: '请输入操作金额', trigger: 'blur' },
				{ pattern: /(^[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^(0){1}$)|(^[0-9]\.[0-9]([0-9])?$)/, message: '请输入正确金额，可保留两位小数', trigger: 'blur' },
			],
			paymentId: [
				{ required: true, message: '请选择支付方式', trigger: 'change' },
			],
		}
	})
	
	//初始化数据
	const initData = async() => {
		//加载支付方式列表
		await useHttp({
			url: `/client/payment/view/10?siteId=${siteConfig.id}&types=pc,native`,
			success(res) {
				datas.paymentList = res.data
			}
		})
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				useHttp({
					method: 'post',
					url: '/account/member/recharge',
					data: datas.model,
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						navigateTo({path: '/payment/confirm', query: {no: res.data.tradeNo}})
						//重置表单
						editFormRef.value.form.resetFields()
					},
					complete() {
						datas.btnLoading = false
					}
				})
			}
		})
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>