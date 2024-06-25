<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员明细'},{title:'充值记录'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="会员账户" v-if="datas.model.id>0">
							{{datas.model.userName}}
						</el-form-item>
						<el-form-item prop="userId" label="会员账户" v-else>
							<dt-member-select v-model="datas.model.userId"></dt-member-select>
						</el-form-item>
						<el-form-item label="支付方式" v-if="datas.model.id>0">
							{{datas.model.paymentTitle}}
						</el-form-item>
						<el-form-item prop="paymentId" label="支付方式" v-else>
							<el-select v-model="datas.model.paymentId" placeholder="请选择...">
								<el-option v-for="item in datas.paymentList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="充值金额" v-if="datas.model.id>0">
							{{datas.model.amount}}
						</el-form-item>
						<el-form-item prop="amount" label="充值金额" v-else>
							<el-input v-model="datas.model.amount" placeholder="必填，要充值的金额">
								<template #append>元</template>
							</el-input>
						</el-form-item>
						<el-form-item label="充值单号" v-if="datas.model.id>0">
							{{datas.model.tradeNo}}
						</el-form-item>
						<el-form-item label="充值时间" v-if="datas.model.id>0">
							{{datas.model.addTime}}
						</el-form-item>
						<el-form-item label="订单状态" v-if="datas.model.id>0">
							<el-tag size="small" type="warning" effect="dark" v-if="datas.model.status==1">已完成</el-tag>
							<el-tag size="small" type="success" effect="dark" v-else-if="datas.model.status==2">已取消</el-tag>
							<el-tag size="small" type="info" effect="dark" v-else>待完成</el-tag>
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm" v-if="!datas.model.id">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtMemberSelect from '../../../components/control/DtMemberSelect.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		paymentList: null, //支付方式
		model: {
			id: 0,
			userId: 0,
			userName: null,
			paymentId: null,
			paymentTitle: null,
			tradeNo: null,
			amount: null,
			status: null,
			addTime: null
		},
		rules: {
			userId: [
				{ required: true, message: '请选择充值账户', trigger: 'blur' }
			],
			amount: [
				{ required: true, message: '请输入操作金额', trigger: 'blur' },
				{ pattern: /(^[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^(0){1}$)|(^[0-9]\.[0-9]([0-9])?$)/, message: '请输入正确金额，可保留两位小数', trigger: 'blur' }
			],
			paymentId: [
				{ required: true, message: '请选择支付方式', trigger: 'change' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/member/recharge/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		} else {
			//加载支付方式列表
			await proxy.$api.request({
				url: '/admin/site/payment/view/0',
				success(res) {
					datas.paymentList = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				proxy.$api.request({
					method: 'post',
					url: `/admin/member/recharge`,
					data: datas.model,
					successMsg: '新增充值记录已成功',
					beforeSend() {
						datas.loading = true
					},
					success(res) {
						proxy.$common.linkUrl('/member/recharge/list') //跳转加列表页
					},
					complete() {
						datas.loading = false
					}
				})
			}
		})
	}
	
	//运行初始化
	initData()
</script>